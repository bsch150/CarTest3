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

    private WheelColliderSource FrontRightWheel;
    private WheelColliderSource FrontLeftWheel;
    private WheelColliderSource BackRightWheel;
    private WheelColliderSource BackLeftWheel;

    public void Start()
    {
        FrontRightWheel = FrontRight.gameObject.AddComponent<WheelColliderSource>();
        FrontLeftWheel = FrontLeft.gameObject.AddComponent<WheelColliderSource>();
        BackRightWheel = BackRight.gameObject.AddComponent<WheelColliderSource>();
        BackLeftWheel = BackLeft.gameObject.AddComponent<WheelColliderSource>();
        Debug.Log("Wheel Radius = " + WheelRadius);
        FrontRightWheel.WheelRadius = WheelRadius;
        FrontLeftWheel.WheelRadius = WheelRadius;
        BackRightWheel.WheelRadius = WheelRadius;
        BackLeftWheel.WheelRadius = WheelRadius;

        FrontRightWheel.SuspensionDistance = SuspensionDistance;
        FrontLeftWheel.SuspensionDistance = SuspensionDistance;
        BackRightWheel.SuspensionDistance = SuspensionDistance;
        BackLeftWheel.SuspensionDistance = SuspensionDistance;
    }

    public void FixedUpdate()
    {
        //Apply the accelerator pedal
        FrontRightWheel.MotorTorque = Input.GetAxis("Vertical") * TorquePerTire;
        FrontLeftWheel.MotorTorque = Input.GetAxis("Vertical") * TorquePerTire;
        if (FourWheelDrive)
        {
            BackRightWheel.MotorTorque = Input.GetAxis("Vertical") * TorquePerTire;
            BackLeftWheel.MotorTorque = Input.GetAxis("Vertical") * TorquePerTire;
        }

        //Turn the steering wheel
        FrontRightWheel.SteerAngle = Input.GetAxis("Horizontal") * 45;
        FrontLeftWheel.SteerAngle = Input.GetAxis("Horizontal") * 45;

        //Apply the hand brake
        if (Input.GetKey(KeyCode.Space))
        {
            BackRightWheel.BrakeTorque = 200000.0f;
            BackLeftWheel.BrakeTorque = 200000.0f;
        }
        else //Remove handbrake
        {
            BackRightWheel.BrakeTorque = 0;
            BackLeftWheel.BrakeTorque = 0;
        }

        if (Input.GetKey(KeyCode.R))
        {
            transform.position = transform.position + Vector3.up;
            transform.rotation = Quaternion.identity;
        }
    }
}
