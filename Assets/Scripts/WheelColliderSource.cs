/******************************************
 * WheelColliderSource
 *  
 * This class was created by:
 * 
 * Nic Tolentino.
 * rotatingcube@gmail.com
 * 
 * I take no liability for it's use and is provided as is.
 * 
 * The classes are based off the original code developed by Unity Technologies.
 * 
 * You are free to use, modify or distribute these classes however you wish, 
 * I only ask that you make mention of their use in your project credits.
*/
using UnityEngine;
using System;
using System.Collections;

public class WheelColliderSource : MonoBehaviour
{
    private Transform m_dummyWheel;
    private Rigidbody m_rigidbody;
    //private SphereCollider m_collider;

    private WheelFrictionCurveSource m_forwardFriction; //Properties of tire friction in the direction the wheel is pointing in.
    private WheelFrictionCurveSource m_sidewaysFriction; //Properties of tire friction in the sideways direction.
    private float m_forwardSlip;
    private float m_sidewaysSlip;
    private Vector3 m_totalForce;
    private Vector3 m_center; //The center of the wheel, measured in the object's local space.
    private Vector3 m_prevPosition;
    private bool m_isGrounded; //Indicates whether the wheel currently collides with something (Read Only).

    private float m_wheelMotorTorque; //Motor torque on the wheel axle. Positive or negative depending on direction.
    private float m_wheelBrakeTorque; //Brake torque. Must be positive.
    private float m_wheelSteerAngle; //Steering angle in degrees, always around the local y-axis.
    private float m_wheelAngularVelocity; //Current wheel axle rotation speed, in rotations per minute (Read Only).
    private float m_wheelRotationAngle;
    private float m_wheelRadius; //The radius of the wheel, measured in local space.
    private float m_wheelMass; //The mass of the wheel. Must be larger than zero.

    private RaycastHit m_raycastHit;
    private float m_suspensionDistance; //Maximum extension distance of wheel suspension, measured in local space.
    private float m_suspensionCompression;
    private float m_suspensionCompressionPrev;
    private JointSpringSource m_suspensionSpring; //The parameters of wheel's suspension. The suspension attempts to reach a target position
    private Vector3 initEuler;
    private float maxWheelVelocity = 5000;
    public GameObject currentlyOn;

    //Debugging color data
    private Color GizmoColor;

    //Standard accessor and mutator properties
    public Vector3 Center
    {
        set
        {
            m_center = value;
            m_dummyWheel.localPosition = transform.localPosition + m_center;
            m_prevPosition = m_dummyWheel.localPosition;
        }
        get
        {
            return m_center;
        }
    }
    public float WheelRadius
    {
        set
        {
            m_wheelRadius = value;
            //Debug.Log("Value = " + value);
            // Debug.Log("m_collider = " + m_collider);
            //m_collider.center = new Vector3(0, m_wheelRadius, 0);
        }
        get
        {
            return m_wheelRadius;
        }
    }
    public float SuspensionDistance
    {
        set
        {
            m_suspensionDistance = value;
        }
        get
        {
            return m_suspensionDistance;
        }
    }
    public JointSpringSource SuspensionSpring
    {
        set
        {
            m_suspensionSpring = value;
        }
        get
        {
            return m_suspensionSpring;
        }
    }
    public float Mass
    {
        set
        {
            m_wheelMass = Mathf.Max(value, 0.0001f);
        }
        get
        {
            return m_wheelMass;
        }
    }
    public WheelFrictionCurveSource ForwardFriction
    {
        set
        {
            m_forwardFriction = value;
        }
        get
        {
            return m_forwardFriction;
        }
    }
    public WheelFrictionCurveSource SidewaysFriction
    {
        set
        {
            m_sidewaysFriction = value;
        }
        get
        {
            return m_sidewaysFriction;
        }
    }
    public float MotorTorque
    {
        set
        {
            m_wheelMotorTorque = value;
        }
        get
        {
            return m_wheelMotorTorque;
        }
    }
    public float BrakeTorque
    {
        set
        {
            m_wheelBrakeTorque = value;
        }
        get
        {
            return m_wheelBrakeTorque;
        }
    }
    public float SteerAngle
    {
        set
        {
            m_wheelSteerAngle = value;
        }
        get
        {
            return m_wheelSteerAngle;
        }
    }
    public bool IsGrounded
    {
        get
        {
            return m_isGrounded;
        }
    }
    public float RPM
    {
        get
        {
            return m_wheelAngularVelocity;
        }
    }

    // Use this for initialization
    public void Awake()
    {
        //Debug.Log("Awake Called");
        m_dummyWheel = new GameObject("DummyWheel").transform;
        m_dummyWheel.transform.parent = this.transform.parent;
        Center = Vector3.zero;
        m_suspensionCompression = 0f;
        Mass = 1.0f;
        m_wheelAngularVelocity = 0;
        m_wheelRotationAngle = 0;

        m_forwardFriction = new WheelFrictionCurveSource();
        m_sidewaysFriction = new WheelFrictionCurveSource();

        m_suspensionSpring = new JointSpringSource();
        m_suspensionSpring.Spring = 100000.0f;
        m_suspensionSpring.Damper = 0f;
        m_suspensionSpring.TargetPosition = 0.0f;
        initEuler = this.transform.localEulerAngles;
        //m_collider = gameObject.AddComponent<SphereCollider>();
    }

    public void Start()
    {

        //Find the rigidbody associated with the wheel
        Transform parent = transform.parent;
        while (parent != null)
        {
            if (parent.gameObject.GetComponent<Rigidbody>() != null)
            {
                m_rigidbody = parent.gameObject.GetComponent<Rigidbody>();
                break;
            }
            parent = parent.parent;
        }

        if (m_rigidbody == null)
        {
            Debug.LogError("WheelColliderSource: Unable to find associated Rigidbody.");
        }
    }

    // Called once per physics update
    public void FixedUpdate()
    {
        UpdateSuspension();

        UpdateWheel();

        if (m_isGrounded)
        {
            CalculateSlips();

            CalculateForcesFromSlips();

            m_rigidbody.AddForceAtPosition(m_totalForce, transform.position);
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position + (m_dummyWheel.up * (m_suspensionDistance - m_suspensionCompression))
        );

        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            transform.position,
            transform.position + (m_dummyWheel.forward * (m_suspensionDistance - m_suspensionCompression))
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            transform.position,
            transform.position + (m_dummyWheel.right * (m_suspensionDistance - m_suspensionCompression))
        );



        //Draw the wheel
        Vector3 point1;
        Vector3 point0 = transform.TransformPoint(m_wheelRadius * new Vector3(0, Mathf.Sin(0), Mathf.Cos(0)));
        for (int i = 1; i <= 20; ++i)
        {
            point1 = transform.TransformPoint(m_wheelRadius * new Vector3(0, Mathf.Sin(i / 20.0f * Mathf.PI * 2.0f), Mathf.Cos(i / 20.0f * Mathf.PI * 2.0f)));
            Gizmos.DrawLine(point0, point1);
            point0 = point1;

        }
        Gizmos.color = Color.white;
    }

    public bool GetGroundHit(out WheelHitSource wheelHit)
    {
        wheelHit = new WheelHitSource();
        if (m_isGrounded)
        {
            wheelHit.Collider = m_raycastHit.collider;
            wheelHit.Point = m_raycastHit.point;
            wheelHit.Normal = m_raycastHit.normal;
            wheelHit.ForwardDir = m_dummyWheel.forward;
            wheelHit.SidewaysDir = m_dummyWheel.right;
            wheelHit.Force = m_totalForce;
            wheelHit.ForwardSlip = m_forwardSlip;
            wheelHit.SidewaysSlip = m_sidewaysSlip;
        }

        return m_isGrounded;
    }
    private void UpdateSuspension()
    {
        //Raycast down along the suspension to find out how far the ground is to the wheel
        bool result = Physics.Raycast(new Ray(m_dummyWheel.position, -m_dummyWheel.up), out m_raycastHit, m_wheelRadius * 10);
        //Debug.Log(m_raycastHit.point);
        if (result)
        { 
            currentlyOn = (m_raycastHit.transform.gameObject);
        }
        if (result && (m_raycastHit.point - transform.position).magnitude <= m_wheelRadius) {
            if ((m_raycastHit.point - transform.position).magnitude <= m_wheelRadius) //The wheel is in contact with the ground
            {
                GizmoColor = Color.green;
                m_isGrounded = true;
            }
        }
        else //The wheel is in the air
        {
            //Debug.Log("In air");
            GizmoColor = Color.blue;
            m_isGrounded = false;
        }
    }
    private void UpdateWheel()
    {
        //Set steering angle of the wheel dummy
        m_dummyWheel.localEulerAngles = new Vector3(0, m_wheelSteerAngle, 0);

        //Calculate the wheel's rotation given it's angular velocity
        m_wheelRotationAngle += m_wheelAngularVelocity * Time.deltaTime;

        //Set the rotation and steer angle of the wheel model
        float tempVeloc = float.IsNaN(m_wheelAngularVelocity) ? 0 : m_wheelRotationAngle;
        Vector3 temp = new Vector3(tempVeloc, m_wheelSteerAngle, 0) + initEuler;
        if (float.IsNaN(temp[0]))
        {
            //Debug.Log ("temp = " + temp);
            //Debug.Log ("m_wheelRotationAngle = " +m_wheelRotationAngle);
        }
        this.transform.localEulerAngles = temp;

        //Set the wheel's position given the current suspension compression
        float temp1 = m_suspensionDistance - m_suspensionCompression;
        transform.localPosition = m_dummyWheel.localPosition - Vector3.up * temp1;

        //Apply rolling force to tires if they are grounded and don't have motor torque applied to them.
        if (m_isGrounded && m_wheelMotorTorque == 0)
        {
            //Apply angular force to wheel from slip
            m_wheelAngularVelocity -= Mathf.Sign(m_forwardSlip) * m_forwardFriction.Evaluate(m_forwardSlip) / (Mathf.PI * 2.0f * m_wheelRadius) / m_wheelMass * Time.deltaTime;
            //Debug.Log("m_wheelAngularVelocity1 = " + m_wheelAngularVelocity);
        }

        //Apply motor torque
        m_wheelAngularVelocity += m_wheelMotorTorque / m_wheelRadius / m_wheelMass * Time.deltaTime;
        m_wheelAngularVelocity = Math.Min(m_wheelAngularVelocity, maxWheelVelocity);
        // Debug.Log("m_wheel vel "+m_wheelAngularVelocity);

        //Apply brake torque
        m_wheelAngularVelocity -= Mathf.Sign(m_wheelAngularVelocity) * Mathf.Min(Mathf.Abs(m_wheelAngularVelocity), m_wheelBrakeTorque * m_wheelRadius / m_wheelMass * Time.deltaTime);
        // Debug.Log("m_wheelAngularVelocity3 = " + m_wheelAngularVelocity);
    }

    private void CalculateSlips()
    {
        //Calculate the wheel's linear velocity
        Vector3 velocity = (m_dummyWheel.position - m_prevPosition) / Time.deltaTime;
        m_prevPosition = m_dummyWheel.position;

        //Store the forward and sideways direction to improve performance
        Vector3 forward = m_dummyWheel.forward;
        Vector3 sideways = -m_dummyWheel.right;

        //Calculate the forward and sideways velocity components relative to the wheel
        Vector3 forwardVelocity = Vector3.Dot(velocity, forward) * forward;
        Vector3 sidewaysVelocity = Vector3.Dot(velocity, sideways) * sideways;

        //Calculate the slip velocities. 
        //Note that these values are different from the standard slip calculation.
        m_forwardSlip = -Mathf.Sign(Vector3.Dot(forward, forwardVelocity)) * forwardVelocity.magnitude + (m_wheelAngularVelocity * Mathf.PI / 180.0f * m_wheelRadius);
        m_sidewaysSlip = -Mathf.Sign(Vector3.Dot(sideways, sidewaysVelocity)) * sidewaysVelocity.magnitude;

    }

    private void CalculateForcesFromSlips()
    {
        //Forward slip force
        m_totalForce = m_dummyWheel.forward * Mathf.Sign(m_forwardSlip) * m_forwardFriction.Evaluate(m_forwardSlip) * 3;

        //Lateral slip force
        m_totalForce -= m_dummyWheel.right * Mathf.Sign(m_sidewaysSlip) * m_forwardFriction.Evaluate(m_sidewaysSlip) * 4;


        //Spring force
        //Debug.Log("Things in 3:\n" + m_dummyWheel.up + "\n" + m_suspensionCompression + "\n" + m_suspensionDistance + "\n" + m_suspensionSpring.TargetPosition + "\n" + m_suspensionSpring.Spring);
        float temp1 = (m_suspensionCompression - m_suspensionDistance * (m_suspensionSpring.TargetPosition));
        //Debug.Log("temp1 = " + temp1);
        m_totalForce += m_dummyWheel.up * temp1 * m_suspensionSpring.Spring;

        //Spring damping force
        m_totalForce += m_dummyWheel.up * (m_suspensionCompression - m_suspensionCompressionPrev) / Time.deltaTime * m_suspensionSpring.Damper;

    }
}
