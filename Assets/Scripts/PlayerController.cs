using UnityEngine;
using System.Collections;
using InputPlusControl;

public class PlayerController : MonoBehaviour {
    private int controllerNum;
    public int playerNum;
    public int finisherTimer = 0;
    private int whichCar;
    private Transform spawnPoint;
    private bool inGarage = false;
    GameObject car;
    public CarController carScript;
    GameObject cameraFab;
    GameObject carCam;
    WorldScript world;
    private int dpadCounter = 0;
    public TrackScript currentTrack = null;
    bool flag = false;
    public GameObject currentlyOn;
    private UIScript ui;
    int[] currentChunk;
    public Color tintColor;
    private float startTime;
    PowerballManager powerballManager;
    SphereCollider chunkCollider;


    public PlayerController(int ctrNum, int carNum, int pNum, Transform spawnP, GameObject cam, WorldScript _world)
    {
        world = _world;
        playerNum = pNum;
        controllerNum = ctrNum;
        whichCar = carNum;
        spawnPoint = spawnP;
        cameraFab = cam;
        ui = new UIScript(world.UIs[0],ctrNum,this,world.players.Count);
        //currentChunk = startChunk;
        initCar();
        powerballManager = new PowerballManager(carScript.PowerBall, this);
    }
    void initChunkCollider()
    {
        chunkCollider = car.AddComponent<SphereCollider>();
        chunkCollider.radius = 1500;
        chunkCollider.isTrigger = true;

    }
    public void leaveTrack()
    {
        currentTrack.finishRace(this);
        setUI(new Vector3(-1, -1, Time.time));
    }
	void Start () {

    }
    public void collideWithSomethingMacro(Collider other)
    {
        int[] coords = world.getCoordsFromString(other.gameObject);
        if (coords[0] != -1 && coords[1] != -1)
        {
            world.chunkHandler.add(other.transform,playerNum);
        }
    }
    public void removeSomethingMacro(Collider other)
    {
        int[] coords = world.getCoordsFromString(other.gameObject);
        if (coords[0] != -1 && coords[1] != -1)
        {
            world.chunkHandler.remove(other.transform,playerNum);
        }
    }

    // Update is called once per frame
    public void updateThis() {
        checkCamToggle();
        checkGarage();
        handleChunkingDiff();
        //Debug.Log("update");
        dpadCounter++;
        finisherTimer++;
        ui.update();
        checkStart();
        powerballManager.act();
        //if(currentlyOn !=  )setTintColor(world.colors.getColor(world.getCoordsFromString(currentlyOn)));
    }
    void handleChunkingDiff()
    {
        if (carScript.currentlyOn != currentlyOn)
        {
            world.doChunks(currentlyOn,carScript.currentlyOn);
            currentlyOn = carScript.currentlyOn;
        }
    }
    void checkGarage()
    {
        if (inGarage && dpadCounter > 15)
        {
            float dpadX = InputPlus.GetData(controllerNum+1, ControllerVarEnum.dpad_right) - InputPlus.GetData(controllerNum+1, ControllerVarEnum.dpad_left);
            if (dpadX < 0)
            {
                //Debug.Log("left pushed");
                whichCar -= 1;
                if (whichCar < 0) whichCar = world.cars.Length - 1;
                initCar();
                dpadCounter = 0;
            }
            else if (dpadX > 0)
            {
                whichCar += 1;
                if (whichCar >= world.cars.Length) whichCar = 0;
                initCar();
                dpadCounter = 0;
            }
            //Debug.Log("dpadx = " + dpadX);
        }
        else
        {
           // Debug.Log("inGarage false");
        }
    }
    void checkCamToggle()
    {
        if (controllerNum != -1)
        {
            if (InputPlus.GetData(controllerNum+1, ControllerVarEnum.ShoulderTop_left) > 0)
            {
                carCam.BroadcastMessage("toggle");
            }
        }
        //carCam.BroadcastMessage("setTarget", (car.transform));
    }
    void checkStart()
    {
        if (InputPlus.GetData(controllerNum + 1, ControllerVarEnum.Interface_right) > 0)
        {
            ui.toggleStart();
        }
    }
    void initCar()
    {
        if (car != null)
        {
            spawnPoint.position = car.transform.position + Vector3.up;
            Destroy(car);
        }
        car = instantiateCarAndPos(world.cars[whichCar], playerNum);

        car.BroadcastMessage ("assignPlayerNumber", playerNum);
        car.BroadcastMessage("enableDrive");
		car.BroadcastMessage("setLastCheckpoint", spawnPoint.gameObject);
        car.BroadcastMessage("setPlayerController", this);

        //car.BroadcastMessage("setTrack", this.gameObject);
        if(carCam == null)carCam = Instantiate (cameraFab);
        car.BroadcastMessage("assignCam", carCam);
        carScript = car.GetComponent<CarController>();
        carCam.BroadcastMessage("setTarget", (car.transform));
        //carCam.transform.SetParent(car.transform);
		//cams[carNum].BroadcastMessage ("assignPlayerNum", carNum+1);
        if (controllerNum != -1)
        {
            car.BroadcastMessage("assignControllerNumber", controllerNum);
            ui.controllerNum = controllerNum;
            car.BroadcastMessage("unfreeze");
        }
        initChunkCollider();

    }
    GameObject instantiateCarAndPos(GameObject c, int num)
    {
        GameObject temp = Instantiate(c);
        //temp.BroadcastMessage("freeze");
        temp.transform.position = spawnPoint.transform.position + new Vector3(-6 + (num * 4), 0, 0);
        temp.transform.rotation = spawnPoint.transform.rotation;
        return temp;

    }
    public void splitCam(int numPlayers)
    {
        if(numPlayers == 2)
        {
            if(playerNum == 1)
            {
                Camera c = carCam.GetComponentInChildren<Camera>();
                c.rect = new Rect(0f, .5f, 1f, .5f);
            }
            else if (playerNum == 2)
            {
                Camera c = carCam.GetComponentInChildren<Camera>();
                c.rect = new Rect(0f, 0f, 1f, .5f);
            }
        }
    }
    public void collideWithPowerball(Powerball behave)
    {
        powerballManager.checkAdd(this, behave);
    }
    public void setControllerNum(int i)
    {
        controllerNum = i;
        car.BroadcastMessage("assignControllerNumber", controllerNum);
        ui.controllerNum = controllerNum;
        car.BroadcastMessage("unfreeze");
    }
    public void setInGarage(bool set)
    {
        inGarage = set;
        //Debug.Log("inGarage = " + inGarage);
    }
    public void setTintColor(Color c)
    {
        ui.setTint(c);
    }
    public void setUI(Vector3 info)
    {
        if(currentTrack != null && !flag)
        {
            flag = true;
            startTime = Time.time;
        }else if(currentTrack == null && flag)
        {
            flag = false;
        }
        else if(currentTrack != null)
        {
            info[2] = Time.time - startTime;
            setHighScoreText("");
        }
        ui.setInfo(info,world.players.Count);
    }
    public void setHighScoreText(string str)
    {
        if (currentTrack == null)
        {
            ui.setHighScoreText(str);
        }
        else
        {
            ui.setHighScoreText("");
        }
    }
    public void fire()
    {
        powerballManager.fire(car.GetComponent<Rigidbody>().velocity);
    }
}
