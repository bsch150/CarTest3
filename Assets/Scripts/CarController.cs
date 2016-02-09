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
using InputPlusControl;

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
    private bool FourWheelDrive = false;
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
    public ParticleSystem Boost;
    public ParticleSystem boostLevel;
	public GameObject cameraPrefab;
    public GameObject PowerBall;

    private WheelColliderSource FrontRightWheel;
    private WheelColliderSource FrontLeftWheel;
    private WheelColliderSource BackRightWheel;
    private WheelColliderSource BackLeftWheel;
    private int numChks;
    private GameObject track;
    private int currentCheckpoint = -1;
    private int currentLap = -1;
    private Text positionText;
    private Text timeText;
    private Text boostText;
    private Text timesText;
    private float boostAmount;
    private float boostUseRate = 10f;
    private float boostGainPerWheel = 1.2f;
    private float maxBoost = 1500;
    private int resetCounter = 0;
    private GameObject toReset;
    private float carTime;
    private float inAirBoostGain = 2;
    private int numLaps;
    private float[] lapTimes;
    private int timeKillCounter = -1; //This is the time I use to grow your race time 
	private int playerNumber;
    private int ctrNum;
	private float startTime;
    private bool invertY = false;
    private bool frozen = true;
    private Vector3 freezePos;
    private Quaternion freezeRot;
    private int resetCountLimit = 50;
    private int fireCount = 0;
    private GameObject cam;
    private float BoostStrength = 80000;
    private float TorquePerTire = 3000;


    private Rigidbody rb;
    
    public void Start(){
        freezePos = this.transform.position;
        freezeRot = this.transform.rotation;
        freeze();
        boostAmount = maxBoost;
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
    }
    void assignCam(GameObject c)
    {
        cam = c;
    }
    void assignControllerNumber(int num)
    {
        Debug.Log("Assigning " + playerNumber + "'s ctrNum to " + (num+1));
        ctrNum = num;
    }

    bool canFire()
    {
        if (fireCount > 100)
        {
            fireCount = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
    void freeze()
    {
        freeze(this.transform);
    }

    void freeze(Transform t)
    {
        freezePos= t.position;
        freezeRot = t.rotation;
        frozen = true;
    }
    void unfreeze()
    {
        frozen = false;
        Debug.Log("p" + playerNumber + " unfrozen?");
    }

    void assignPlayerNumber(int num){
		playerNumber = num;
	}
	string getAxisString(string str){
		return str + playerNumber.ToString ();
	}
    void assignNumLapsAndChks(Vector2 num)
    {
		numLaps = (int)num[0];
            lapTimes = new float[numLaps];
		numChks = (int)num [1];
    }
    void assignNumChks(int num)
    {
        numChks = num;
    }
    void setTrack(GameObject t)
    {
        track = t;
    }
    //void setUI(GameObject t)
    //{
     //   UI = t;
   // }
	void checkNewLap(GameObject t){
		if (currentLap == -1) {
			currentLap = 0;
			currentCheckpoint = 0;
			track = t;
			startTime = Time.time;
			track.BroadcastMessage("respondWithNumLaps",playerNumber);
            track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));
        } else {
			track.BroadcastMessage("checkNewLap",new Vector3(playerNumber,currentLap,currentCheckpoint));
        }
	}
	void confirmNewLap(){
		//lapTimes[currentLap] = carTime;
		Debug.Log ("confirmed new Lap");
		lapTimes [currentLap] = Time.time - startTime;
		currentLap++;
		currentCheckpoint = 0;
        track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));

    }
    void setLastCheckpoint(GameObject chk)
    {
        //Debug.Log("Tset");
        toReset = chk;
    }
    void reset()
    {
        RaycastHit m_raycastHit;
        bool result = Physics.Raycast(new Ray(toReset.transform.position, -Vector3.up), out m_raycastHit);
        if (result) {
            this.transform.position = m_raycastHit.point;
           // Debug.Log("teset");
            this.transform.rotation = toReset.transform.rotation;
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
    void checkCheckpoint(int num)
    { 
            if (num == currentCheckpoint + 1)
            {
                currentCheckpoint++;
			Debug.Log ("currentCheck =  " + currentCheckpoint);
            // [0] is the car's playernumber, [1] is the cars lap and [2] is which checkPoint
            track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));
		
            }else{
			Debug.Log("num == " + num);
			}
       // }
    }
    void setTimeText(double time)
    {
        if (timeKillCounter > 0)
        {
            timeKillCounter--;
            float temp = 0;
            for (int i = 0; i < numLaps; i++)
            {
				temp += lapTimes[i];
            }
            //Debug.Log("setting carTime to " + temp);
            carTime = temp;
        }
        else if(currentLap >= 0) {
            carTime = (((float)(Math.Truncate(time * 100))) / 100);
        }
    }
    void checkReset(float resetButton)
    {
        if (resetButton > 0)//Reset button pushed down
        {
            resetCounter++;
            if(resetCounter >= resetCountLimit)//player has been holding the reset button for 'long enough'
            {
                reset();
            }
        }
        else//reset the resetCounter
        {
            resetCounter = 0;
        }
    }
    void cameraControl(Vector2 rightThumb)
    {
        cam.BroadcastMessage("setRotateOn",rightThumb);
    }
    void ApplyControls(float acc, float hAxis, float vAxis, float boost, float EBrake, float jump, float AxisToggle,float resetButton,float fireButton,Vector2 rightThumb)
    {
        cameraControl(rightThumb);
        checkReset(resetButton);
        
        FrontRightWheel.MotorTorque = acc * TorquePerTire;
        FrontLeftWheel.MotorTorque = acc * TorquePerTire;
        if (FourWheelDrive)
        {
            BackRightWheel.MotorTorque = acc * TorquePerTire;
            BackLeftWheel.MotorTorque = acc * TorquePerTire;
        }

        //Turn the steering wheel
        FrontRightWheel.SteerAngle = hAxis * 35;
        FrontLeftWheel.SteerAngle = hAxis * 35;

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
        var em = Boost.emission;
        Boost.startColor = boostLevel.startColor;
        em.enabled = false;
        if (boost > 0)
        {
            if (boostAmount > 0)
            {
                em.enabled = true;
                rb.AddForce(transform.TransformDirection(Vector3.forward * BoostStrength));
                boostAmount -= boostUseRate;
            }
            else
            {
            }
        }
        else
        {

        }



        Jump(jump, hAxis, vAxis, AxisToggle);
        if (fireButton > 0 && canFire())
        {
            var pb = Instantiate(PowerBall);
            pb.transform.position = this.transform.position + new Vector3(0, 3, 0);
            pb.GetComponent<Rigidbody>().velocity = transform.forward * (20) + rb.velocity;
        }
    }
    void finishTrack()
    {
        currentLap = -1;
        currentCheckpoint = -1;
        timeKillCounter = 1000;
		float sum = 0f;
		for (int i = 0; i < lapTimes.Length; i++) {
			sum += lapTimes[i];
		}
		Debug.Log ("Player " + playerNumber + " finished, time = " + sum + ", lapTime len = " + lapTimes.Length);
    }
    public void FixedUpdate()
    {
        if (!frozen)
        {
            if (canDrive)
            {
                //Apply the accelerator pedal
                float acc = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderBottom_right) - InputPlus.GetData(ctrNum + 1 , ControllerVarEnum.ShoulderBottom_left));//Input.GetAxis (getAxisString ("Accelerate")));
                Debug.Log("acc = " + acc);
                float hAxis = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbLeft_x));//Input.GetAxis (getAxisString ("Horizontal"));
                float vAxis = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbLeft_y));//Input.GetAxis (getAxisString ("Vertical"));
                if (invertY)
                {
                    vAxis = -vAxis;
                }
                if (canDrive)
                {
                    //ApplyControls (acc, hAxis, vAxis, Input.GetAxis (getAxisString ("Boost")), Input.GetAxis (getAxisString ("EBrake")), Input.GetAxis (getAxisString ("Jump")), Input.GetAxis (getAxisString ("AxisToggle")));
                    ApplyControls(acc, hAxis, vAxis, 
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_bottom),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_left),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderTop_right),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_left),
                        InputPlus.GetData(ctrNum + 1,ControllerVarEnum.FP_top),
                        InputPlus.GetData(ctrNum + 1,ControllerVarEnum.FP_right),
                        new Vector2(InputPlus.GetData(ctrNum + 1,ControllerVarEnum.ThumbRight_x), InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbRight_x)));
                }
                //Debug.Log(Input.GetAxis(getAxisString("Vertical")));
                //setUIText();
                if (boostAmount > 500)
                {
                    boostLevel.startColor = Color.Lerp(Color.blue, Color.yellow, ((float)boostAmount - (maxBoost / 2)) / ((float)maxBoost / 2));
                }
                else {
                    boostLevel.startColor = Color.Lerp(Color.red, Color.blue, ((float)boostAmount) / ((float)maxBoost / 2));
                }
                boostLevel.startLifetime = Remap(boostAmount, 0, maxBoost, 0f, .45f);
            }
            else {
                Debug.Log("canDrive is false");
                BackLeftWheel.BrakeTorque = 200000.0f;
                BackRightWheel.BrakeTorque = 200000.0f;
            }
        }
        else
        {
            Debug.Log("p "+playerNumber+", ctr "+ctrNum+" frozen!");
            if(ctrNum > -1)
            {
                unfreeze();
            }
            this.transform.position = freezePos;
            this.transform.rotation = freezeRot;
        }
        fireCount++;
    }
    float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    void enableDrive()
    {
        canDrive = true;
		
    }
    void enableDrive(bool flag)
    {
        canDrive = flag;

    }
    private void Jump(float jump, float hAxis, float vAxis, float axisToggle)
    {
        int cnt = 0;
        if (BackLeftWheel.IsGrounded) cnt += 1;
        if (BackRightWheel.IsGrounded) cnt += 1;
        if (FrontLeftWheel.IsGrounded) cnt += 1;
        if (FrontRightWheel.IsGrounded) cnt += 1;
        rb.AddForce(Vector3.up * (cnt * jump*JumpStrength));
        boostAmount += (cnt * boostGainPerWheel)+inAirBoostGain;
        if (boostAmount > maxBoost) boostAmount = maxBoost;
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
