/******************************************
 * CarController
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
using System.Collections;

public class CarController : MonoBehaviour
{
    public Transform FrontRight;
    public Transform FrontLeft;
    public Transform BackRight;
    public Transform BackLeft;
    [SerializeField]
    private float WheelRadius;
    [SerializeField]
    private float SuspensionDistance;
    [SerializeField]
    private float TorquePerTire;
    [SerializeField]
    private bool FourWheelDrive = false;
    [SerializeField]
    private float BoostStrength;
    [SerializeField]
    private float JumpStrength;
    [SerializeField]
    private float m_XRotationSpeed = 5000;
    [SerializeField]
    private float m_YRotationSpeed = 5000;
    [SerializeField]
    private float m_ZRotationSpeed = 5000;

    private WheelColliderSource FrontRightWheel;
    private WheelColliderSource FrontLeftWheel;
    private WheelColliderSource BackRightWheel;
    private WheelColliderSource BackLeftWheel;

    private Rigidbody rb;

    public void Start()
    {
        FrontRightWheel = FrontRight.gameObject.AddComponent<WheelColliderSource>();
        FrontLeftWheel = FrontLeft.gameObject.AddComponent<WheelColliderSource>();
        BackRightWheel = BackRight.gameObject.AddComponent<WheelColliderSource>();
        BackLeftWheel = BackLeft.gameObject.AddComponent<WheelColliderSource>();
        //Debug.Log("Wheel Radius = " + WheelRadius);
        FrontRightWheel.WheelRadius = WheelRadius;
        FrontLeftWheel.WheelRadius = WheelRadius;
        BackRightWheel.WheelRadius = WheelRadius;
        BackLeftWheel.WheelRadius = WheelRadius;

        FrontRightWheel.SuspensionDistance = SuspensionDistance;
        FrontLeftWheel.SuspensionDistance = SuspensionDistance;
        BackRightWheel.SuspensionDistance = SuspensionDistance;
        BackLeftWheel.SuspensionDistance = SuspensionDistance;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass - (Vector3.up * 0.5f);
    }

    public void FixedUpdate()
    {
        //Apply the accelerator pedal
        float acc = (Input.GetAxis("Accelerate"));
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        //Debug.Log(Input.GetAxis("Vertical"));
        FrontRightWheel.MotorTorque = acc * TorquePerTire;
        FrontLeftWheel.MotorTorque = acc * TorquePerTire;
        if (FourWheelDrive)
        {
            BackRightWheel.MotorTorque = acc * TorquePerTire;
            BackLeftWheel.MotorTorque = acc * TorquePerTire;
        }

        //Turn the steering wheel
        FrontRightWheel.SteerAngle = hAxis * 45;
        FrontLeftWheel.SteerAngle = hAxis * 45;

        //Apply the hand brake
        if (Input.GetAxis("EBrake") > 0)
        {
            BackRightWheel.BrakeTorque = 200000.0f;
            BackLeftWheel.BrakeTorque = 200000.0f;
        }
        else //Remove handbrake
        {
            BackRightWheel.BrakeTorque = 0;
            BackLeftWheel.BrakeTorque = 0;
        }
        if(Input.GetAxis("Boost") > 0)
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward * BoostStrength));
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.position = transform.position + Vector3.up;
            transform.rotation = Quaternion.identity;
        }
        Jump(Input.GetAxis("Jump"), hAxis, vAxis, Input.GetAxis("AxisToggle"));
    }
    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    private void Jump(float jump, float hAxis, float vAxis, float axisToggle)
    {
        int cnt = 0;
        if (BackLeftWheel.IsGrounded) cnt += 1;
        if (BackRightWheel.IsGrounded) cnt += 1;
        if (FrontLeftWheel.IsGrounded) cnt += 1;
        if (FrontRightWheel.IsGrounded) cnt += 1;
        rb.AddForce(Vector3.up * (cnt * jump*JumpStrength));
        if (cnt <= 3)
        {
            Vector3 toAdd = new Vector3(Remap(vAxis, 1, -1, -m_XRotationSpeed, m_XRotationSpeed), 0, 0);
            if (axisToggle == 1)
            {
                float value  = Remap(hAxis, -1, 1, m_ZRotationSpeed, -m_ZRotationSpeed);
                //Debug.Log(vAxis);
                toAdd[2] = value;
            }
            else
            {
                //Debug.Log(toAdd);
                toAdd[1] = Remap(hAxis, -1, 1, -m_YRotationSpeed, m_YRotationSpeed);
            }
            rb.AddRelativeTorque(toAdd);
        }
    }
}
