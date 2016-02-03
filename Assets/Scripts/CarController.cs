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
using UnityEngine.UI;
using System.Collections;
using  System;

public class CarController : MonoBehaviour
{
    public Transform FrontRight;
    public Transform FrontLeft;
    public Transform BackRight;
    public Transform BackLeft;

    [SerializeField]
    private float FrontWheelRadius;
    [SerializeField]
    private float BackWheelRadius;
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
    [SerializeField]
    private bool canDrive = false;

    private WheelColliderSource FrontRightWheel;
    private WheelColliderSource FrontLeftWheel;
    private WheelColliderSource BackRightWheel;
    private WheelColliderSource BackLeftWheel;
    private int numChks;
    private Transform track;
    private int currentCheckpoint = -1;
    private int currentLap = -1;
    private GameObject UI;
    private Text positionText;
    private Text timeText;


    private Rigidbody rb;

    public void Start()
    {
        FrontRightWheel = FrontRight.gameObject.AddComponent<WheelColliderSource>();
        FrontLeftWheel = FrontLeft.gameObject.AddComponent<WheelColliderSource>();
        BackRightWheel = BackRight.gameObject.AddComponent<WheelColliderSource>();
        BackLeftWheel = BackLeft.gameObject.AddComponent<WheelColliderSource>();
        //GameObject
        //FrontRight.gameObject.
        //Debug.Log("Wheel Radius = " + WheelRadius);
        FrontRightWheel.WheelRadius = FrontWheelRadius;
        FrontLeftWheel.WheelRadius = FrontWheelRadius;
        BackRightWheel.WheelRadius = BackWheelRadius;
        BackLeftWheel.WheelRadius = BackWheelRadius;

        FrontRightWheel.SuspensionDistance = SuspensionDistance;
        FrontLeftWheel.SuspensionDistance = SuspensionDistance;
        BackRightWheel.SuspensionDistance = SuspensionDistance;
        BackLeftWheel.SuspensionDistance = SuspensionDistance;

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass - (Vector3.up * 0.5f);

        Text[] temp = UI.GetComponentsInChildren<Text>();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].name == "Position")
                positionText = temp[i];
            else if (temp[i].name == "Time")
                timeText = temp[i];
        }
    }
    void setNumChks(int num)
    {
        numChks = num;
    }
    void setTrack(Transform t)
    {
        track = t;
    }
    void setUI(GameObject t)
    {
        UI = t;
    }
    void checkCheckpoint(int num)
    { 
        //Debug.Log("Got cehckCheck");
        if (currentCheckpoint == num - 1 || (currentCheckpoint == numChks - 1 && num == 0))
        {
            if(num == 0)
            {
                currentLap++;
            }
            Debug.Log("doing check");
            currentCheckpoint++;
            if(currentCheckpoint == numChks)
            {
                currentCheckpoint = 0;
            }
            positionText.text = currentLap.ToString() + ", " + currentCheckpoint.ToString();
        }
        else
        {

        }
    }
    void setTimeText(double time)
    {
        string toDisplay = (((double)(Math.Truncate(time * 100)))/100).ToString();
        
        timeText.text = toDisplay;
    }
    void ApplyControls(float acc, float hAxis, float vAxis, float boost, float EBrake, float jump, float AxisToggle)
    {
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
        if (EBrake > 0)
        {
            BackRightWheel.BrakeTorque = 200000.0f;
            BackLeftWheel.BrakeTorque = 200000.0f;
        }
        else //Remove handbrake
        {
            BackRightWheel.BrakeTorque = 0;
            BackLeftWheel.BrakeTorque = 0;
            FrontRightWheel.BrakeTorque = 0;
            FrontLeftWheel.BrakeTorque = 0;
        }
        if (boost > 0)
        {
            rb.AddForce(transform.TransformDirection(Vector3.forward * BoostStrength));
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.position = transform.position + Vector3.up;
            transform.rotation = Quaternion.identity;
        }
        Jump(jump, hAxis, vAxis, AxisToggle);
    }
    public void FixedUpdate()
    {
        //Apply the accelerator pedal
        float acc = (Input.GetAxis("Accelerate"));
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        vAxis = -vAxis;
        if (canDrive)
        {
            ApplyControls(acc, hAxis, vAxis, Input.GetAxis("Boost"), Input.GetAxis("EBrake"), Input.GetAxis("Jump"), Input.GetAxis("AxisToggle"));
        }
        else
        {
            BackLeftWheel.BrakeTorque = 200000.0f;
            BackRightWheel.BrakeTorque = 200000.0f;
        }
        //Debug.Log(Input.GetAxis("Vertical"));

    }
    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    void enableDrive()
    {
        canDrive = true;
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
