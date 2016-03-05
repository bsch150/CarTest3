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
    GameObject cameraFab;
    GameObject carCam;
    WorldScript world;
    private int dpadCounter = 0;
    public string currentTrack = "NONE";
    private UIScript ui;
    
    public PlayerController(int ctrNum, int carNum, int pNum, Transform spawnP, GameObject cam, WorldScript _world)
    {
        world = _world;
        playerNum = pNum;
        controllerNum = ctrNum;
        whichCar = carNum;
        spawnPoint = spawnP;
        cameraFab = cam;
        ui = new UIScript(world.UIs[playerNum - 1]);
        initCar();
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	public void updateThis() {
        checkCamToggle();
        checkGarage();
        //Debug.Log("update");
        dpadCounter++;
        finisherTimer++;
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
        carCam.BroadcastMessage("setTarget", (car.transform));
        //carCam.transform.SetParent(car.transform);
		//cams[carNum].BroadcastMessage ("assignPlayerNum", carNum+1);
        if (controllerNum != -1)
        {
            car.BroadcastMessage("assignControllerNumber", controllerNum);
            car.BroadcastMessage("unfreeze");
        }

    }
    GameObject instantiateCarAndPos(GameObject c, int num)
    {
        GameObject temp = Instantiate(c);
        //temp.BroadcastMessage("freeze");
        temp.transform.position = spawnPoint.transform.position + new Vector3(-6 + (num * 4), 0, 0);
        temp.transform.rotation = spawnPoint.transform.rotation;
        return temp;

    }
    public void setControllerNum(int i)
    {
        controllerNum = i;
        car.BroadcastMessage("assignControllerNumber", controllerNum);
        car.BroadcastMessage("unfreeze");
    }
    public void setInGarage(bool set)
    {
        inGarage = set;
        //Debug.Log("inGarage = " + inGarage);
    }
    public void setUI(Vector2 info)
    {
        ui.setInfo(info);
    }
}
