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
    //public variables to be set in GUI
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
    private bool FourWheelDrive = true; //I'm not certain this doesa anything
    [SerializeField]
    private float JumpStrength; //I would want to put this private to maintain balance but some cars seem to need more. (The Comanche right now)
    [SerializeField]
    private float m_XRotationSpeed = 5000;
    [SerializeField]
    private float m_YRotationSpeed = 5000;
    [SerializeField]
    private float m_ZRotationSpeed = 5000;
    [SerializeField]
    private bool canDrive = false;//Hmmmmmm I don't know about this one (Maybe it doesn't do anythign either
    public ParticleSystem Boost;
    public ParticleSystem boostLevel;
	public GameObject cameraPrefab;
    public GameObject PowerBall;
    public Material primaryMaterial;
    public Material secondaryMaterial;

    //private variables for wheels
    private WheelColliderSource FrontRightWheel;
    private WheelColliderSource FrontLeftWheel;
    private WheelColliderSource BackRightWheel;
    private WheelColliderSource BackLeftWheel;

    //private variables for the track system
    private int numChks;
    private GameObject track;
    private int currentCheckpoint = -1;
    private int currentLap = -1;
    private float carTime;
    private int numLaps;
	private float startTime;
    private float[] lapTimes;

    //private variables for the UI system TODO: take this out, I'm going about it a different way. [UI contained in worldScript]
    private Text positionText;
    private Text timeText;
    private Text boostText;
    private Text timesText;
    private int timeKillCounter = -1;  

    //private variables for the boost system and driving mechanics
    private float boostAmount;
    private float boostUseRate = 30f;
    private float boostGainPerWheel = 4f;
    private float maxBoost = 1500;
    private float inAirBoostGain = 2;
    private float BoostStrength = 60000;
    private float TorquePerTire = 3000;

    //for reset
    private int resetCounter = 0;
    private GameObject toReset;
    private int resetCountLimit = 50;

    //for identity and code safety (and weapons for now because there's only one variable for that right now)
    private int playerNumber;
    private int ctrNum;
    private bool invertY = false;
    private bool frozen = true;
    private Vector3 freezePos;
    private Quaternion freezeRot;
    private int fireCount = 0;
    private GameObject cam;


    private Rigidbody rb;
    
    public void Start(){
        //initialize freeze (freeze is a quick fix for handling controller variability
        freezePos = this.transform.position;
        freezeRot = this.transform.rotation;
        freeze();
        //boost and wheels init
        boostAmount = maxBoost;
        FrontRightWheel = FrontRight.gameObject.AddComponent<WheelColliderSource>();
        FrontLeftWheel = FrontLeft.gameObject.AddComponent<WheelColliderSource>();
        BackRightWheel = BackRight.gameObject.AddComponent<WheelColliderSource>();
        BackLeftWheel = BackLeft.gameObject.AddComponent<WheelColliderSource>();
        FrontRightWheel.WheelRadius = FrontWheelRadius;
        FrontLeftWheel.WheelRadius = FrontWheelRadius;
        BackRightWheel.WheelRadius = BackWheelRadius;
        BackLeftWheel.WheelRadius = BackWheelRadius;

        FrontRightWheel.SuspensionDistance = SuspensionDistance;
        FrontLeftWheel.SuspensionDistance = SuspensionDistance;
        BackRightWheel.SuspensionDistance = SuspensionDistance;
        BackLeftWheel.SuspensionDistance = SuspensionDistance;

        //set rb (Rigibody associated weith the car)
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass - (Vector3.up * 0.5f);

        //set Boost colors;
        setBoostGrad();
    }
    void setBoostGrad()
    {
        var col = Boost.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        var gck = new GradientColorKey[3];
        gck[0].color = boostLevel.startColor;
        gck[0].time = 0.0f;
        gck[1].color = primaryMaterial.color;
        gck[1].time = 0.1f;
        gck[2].color = secondaryMaterial.color;
        gck[2].time = 1.0f;
        var gak = new GradientAlphaKey[2];
        gak[0].alpha = 1.0f;
        gak[0].time = 0.0f;
        gak[1].alpha = 1.0f;
        gak[1].time = 1.0f;
        grad.SetKeys(gck, gak);

        col.color = new ParticleSystem.MinMaxGradient(grad);
    }
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
    void cameraControl(float camToggle, Vector2 rightThumb)
    {
        if (cam != null)
        {
            if (camToggle > 0)
            {
                cam.BroadcastMessage("toggle");
            }
            cam.BroadcastMessage("setThumbsticks", rightThumb);
        }
    }
    void ApplyControls(float acc, float hAxis, float vAxis, float boost, float EBrake, float jump, float AxisToggle,float resetButton,float fireButton,Vector2 rightThumb,float cameraToggle)
    {
        cameraControl(cameraToggle,rightThumb);
        checkReset(resetButton);
        
        FrontRightWheel.MotorTorque = acc * TorquePerTire;
        FrontLeftWheel.MotorTorque = acc * TorquePerTire;
        if (FourWheelDrive)
        {
            BackRightWheel.MotorTorque = acc * TorquePerTire;
            BackLeftWheel.MotorTorque = acc * TorquePerTire;
        }

        //Turn the steering wheel
        FrontRightWheel.SteerAngle = hAxis * 15;
        FrontLeftWheel.SteerAngle = hAxis * 15;

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
                //float acc = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderBottom_right) - InputPlus.GetData(ctrNum + 1 , ControllerVarEnum.ShoulderBottom_left));//Input.GetAxis (getAxisString ("Accelerate")));
                float forward = InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderBottom_right); 
                float backward = InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderBottom_left);
                Debug.Log("forward = " + forward);
                Debug.Log("backward = " + backward);
                float acc = forward - backward;
                float hAxis = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbLeft_x));
                float vAxis = (InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbLeft_y));
                if (invertY)
                {
                    vAxis = -vAxis;
                }
                if (canDrive)
                {
                    ApplyControls(acc, hAxis, vAxis, 
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_bottom),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_left),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderTop_right),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_left),
                        InputPlus.GetData(ctrNum + 1,ControllerVarEnum.FP_top),
                        InputPlus.GetData(ctrNum + 1,ControllerVarEnum.FP_right),
                        new Vector2(InputPlus.GetData(ctrNum + 1,ControllerVarEnum.ThumbRight_x), InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbRight_y)),
                        InputPlus.GetData(ctrNum + 1,ControllerVarEnum.ShoulderTop_left));
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
        setBoostGrad();
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

    void assignCam(GameObject c)
    {
        cam = c;
    }
    void assignControllerNumber(int num)
    {
        Debug.Log("Assigning " + playerNumber + "'s ctrNum to " + (num + 1));
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
        freezePos = t.position;
        freezeRot = t.rotation;
        frozen = true;
    }
    void unfreeze()
    {
        frozen = false;
        Debug.Log("p" + playerNumber + " unfrozen?");
    }

    void assignPlayerNumber(int num)
    {
        playerNumber = num;
    }
    string getAxisString(string str)
    {
        return str + playerNumber.ToString();
    }
    void assignNumLapsAndChks(Vector2 num)
    {
        numLaps = (int)num[0];
        lapTimes = new float[numLaps];
        numChks = (int)num[1];
    }
    void assignNumChks(int num)
    {
        numChks = num;
    }
    void setTrack(GameObject t)
    {
        track = t;
    }
    void randomizeColors() {


        System.Random r = new System.Random();
        Color cP = new Color((float)(r.NextDouble()), (float)(r.NextDouble() ), (float)(r.NextDouble() ));
        Color cS = new Color((float)(r.NextDouble() * 255), (float)(r.NextDouble() ), (float)(r.NextDouble() ));
        primaryMaterial.shader = Shader.Find("Standard");
        secondaryMaterial.shader = Shader.Find("Standard");
        primaryMaterial.SetColor("_Color", cP);
        secondaryMaterial.SetColor("_Color", cS);
        Debug.Log("randomizing colors?");

    }
}
