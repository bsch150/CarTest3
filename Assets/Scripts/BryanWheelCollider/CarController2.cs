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
using System;
using InputPlusControl;

public class CarController2 : MonoBehaviour
{
    //public variables to be set in GUI
    //These Transforms are just locations of the wheels. The rotation and scale of the Tranforms are important as well.
    public Transform FLWheelPos;
    public Transform FRWheelPos;
    public Transform BLWheelPos;
    public Transform BRWheelPos;
    //Scripts assigned to the wheels.
    private WheelCollider2 FL;
    private WheelCollider2 FR;
    private WheelCollider2 BL;
    private WheelCollider2 BR;

    public float FrontWheelRadius;
    public float BackWheelRadius;
    public float SuspensionDistance;
    public float JumpStrength; //I would want to put this private to maintain balance but some cars seem to need more. (The Comanche right now)
    public float m_XRotationSpeed = 5000;
    public float m_YRotationSpeed = 5000;
    public float m_ZRotationSpeed = 5000;
    public bool canDrive = false;//Hmmmmmm I don't know about this one (Maybe it doesn't do anythign either
    public ParticleSystem Boost;
    public ParticleSystem boostLevel;
    public GameObject cameraPrefab;
    public GameObject PowerBall;
    public Material primaryMaterial;
    public Material secondaryMaterial;


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
    private float boostUseRate = 10f;
    private float boostGainPerWheel = 2f;
    private float maxBoost = 1500;
    private float inAirBoostGain = 2;
    private float BoostStrength = 70000;
    private float TorquePerTire = 3000;
    private bool FourWheelDrive = true; //I'm not certain this doesa anything

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
    private float maxRT = -100;
    private float minRT = 100;
    private bool debugMode = false;

    void print(string str)
    {
        if (debugMode) Debug.Log(str);
    }
    private Rigidbody rb;
    void initWheelColliders()
    {
        FL = FLWheelPos.gameObject.GetComponent<WheelCollider2>();
        FR = FRWheelPos.gameObject.GetComponent<WheelCollider2>();
        BL = BLWheelPos.gameObject.GetComponent<WheelCollider2>();
        BR = FRWheelPos.gameObject.GetComponent<WheelCollider2>();
        FL.assignWheelRadius(FrontWheelRadius);
        FR.assignWheelRadius(FrontWheelRadius);
        BL.assignWheelRadius(BackWheelRadius);
        BR.assignWheelRadius(BackWheelRadius);
        FL.assignCarRigidBody(rb);
        FR.assignCarRigidBody(rb);
        BL.assignCarRigidBody(rb);
        BR.assignCarRigidBody(rb);
    }
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = rb.centerOfMass - (Vector3.up * 0.5f);
        initWheelColliders();
        //initialize freeze (freeze is a quick fix for handling controller variability
        freezePos = this.transform.position;
        freezeRot = this.transform.rotation;
        freeze();
        //boost and wheels init
        boostAmount = maxBoost;

        //set rb (Rigibody associated weith the car)

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
    void checkNewLap(GameObject t)
    {
        if (currentLap == -1)
        {
            currentLap = 0;
            currentCheckpoint = 0;
            track = t;
            startTime = Time.time;
            track.BroadcastMessage("respondWithNumLaps", playerNumber);
            track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));
        }
        else {
            track.BroadcastMessage("checkNewLap", new Vector3(playerNumber, currentLap, currentCheckpoint));
        }
    }
    void confirmNewLap()
    {
        //lapTimes[currentLap] = carTime;
        print("confirmed new Lap");
        lapTimes[currentLap] = Time.time - startTime;
        currentLap++;
        currentCheckpoint = 0;
        track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));

    }
    void setLastCheckpoint(GameObject chk)
    {
        //Print("Tset");
        toReset = chk;
    }
    void reset()
    {
        RaycastHit m_raycastHit;
        bool result = Physics.Raycast(new Ray(toReset.transform.position, -Vector3.up), out m_raycastHit);
        if (result)
        {
            this.transform.position = m_raycastHit.point;
            // Print("teset");
            this.transform.rotation = toReset.transform.rotation;
            rb.velocity = new Vector3(0, 0, 0);
        }
    }
    void checkCheckpoint(int num)
    {
        if (num == currentCheckpoint + 1)
        {
            currentCheckpoint++;
            print("currentCheck =  " + currentCheckpoint);
            // [0] is the car's playernumber, [1] is the cars lap and [2] is which checkPoint
            track.BroadcastMessage("checkCheckpoint", new Vector3(playerNumber, currentLap, currentCheckpoint));

        }
        else {
            print("num == " + num);
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
            //print("setting carTime to " + temp);
            carTime = temp;
        }
        else if (currentLap >= 0)
        {
            carTime = (((float)(Math.Truncate(time * 100))) / 100);
        }
    }
    void checkReset(float resetButton)
    {
        if (resetButton > 0)//Reset button pushed down
        {
            resetCounter++;
            if (resetCounter >= resetCountLimit)//player has been holding the reset button for 'long enough'
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
           // cam.BroadcastMessage("setThumbsticks", rightThumb);
        }
    }
    void ApplyControls(float acc, float hAxis, float vAxis, float boost, float EBrake, float jump, float AxisToggle, float resetButton, float fireButton, Vector2 rightThumb, float cameraToggle)
    {
        cameraControl(cameraToggle, rightThumb);
        checkReset(resetButton);
        FR.move(acc);
        FL.move(acc);
        if (FourWheelDrive)
        {
            BR.move(acc);
            BL.move(acc);
        }

        //Turn the steering wheel

        //Apply the hand brake
        if (EBrake > 0)
        {
        }
        else //Remove handbrake
        {
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
        for (int i = 0; i < lapTimes.Length; i++)
        {
            sum += lapTimes[i];
        }
        print("Player " + playerNumber + " finished, time = " + sum + ", lapTime len = " + lapTimes.Length);
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
                minRT = Math.Min(forward, minRT);
                maxRT = Math.Max(forward, maxRT);
                forward = Remap(forward, minRT, maxRT, 0, 1);
                float backward = InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderBottom_left);
                print("minRT = " + minRT);
                print("maxRT = " + maxRT);
                print("forward = " + forward);
                print("backward = " + backward);
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
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_top),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.FP_right),
                        new Vector2(InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbRight_x), InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ThumbRight_y)),
                        InputPlus.GetData(ctrNum + 1, ControllerVarEnum.ShoulderTop_left));
                }
                //print(Input.GetAxis(getAxisString("Vertical")));
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
                print("canDrive is false");
            }
        }
        else
        {
            print("p " + playerNumber + ", ctr " + ctrNum + " frozen!");
            if (ctrNum > -1)
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
        if (BL.IsGrounded()) cnt += 1;
        if (BR.IsGrounded()) cnt += 1;
        if (FL.IsGrounded()) cnt += 1;
        if (FR.IsGrounded()) cnt += 1;
        rb.AddForce(Vector3.up * (cnt * jump * JumpStrength));
        boostAmount += (cnt * boostGainPerWheel) + inAirBoostGain;
        if (boostAmount > maxBoost) boostAmount = maxBoost;
        if (cnt <= 3)
        {
            Vector3 toAdd = new Vector3(Remap(vAxis, 1, -1, -m_XRotationSpeed, m_XRotationSpeed), 0, 0);
            if (axisToggle == 1)
            {
                float value = Remap(hAxis, -1, 1, m_ZRotationSpeed, -m_ZRotationSpeed);
                //print(vAxis);
                toAdd[2] = value;
            }
            else
            {
                //print(toAdd);
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
        print("Assigning " + playerNumber + "'s ctrNum to " + (num + 1));
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
        print("p" + playerNumber + " unfrozen?");
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
    void randomizeColors()
    {


        System.Random r = new System.Random();
        Color cP = new Color((float)(r.NextDouble()), (float)(r.NextDouble()), (float)(r.NextDouble()));
        Color cS = new Color((float)(r.NextDouble() * 255), (float)(r.NextDouble()), (float)(r.NextDouble()));
        primaryMaterial.shader = Shader.Find("Standard");
        secondaryMaterial.shader = Shader.Find("Standard");
        primaryMaterial.SetColor("_Color", cP);
        secondaryMaterial.SetColor("_Color", cS);
        print("randomizing colors?");

    }
}
