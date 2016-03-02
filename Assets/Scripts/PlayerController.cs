using UnityEngine;
using System.Collections;
using InputPlusControl;

public class PlayerController : MonoBehaviour {
    private int controllerNum;
    private int playerNum;
    private int whichCar;
    private Transform spawnPoint;
    GameObject car;
    GameObject cameraFab;
    GameObject carCam;
    GameObject[] cars;
	// Use this for initialization
    public PlayerController(int ctrNum, int carNum, GameObject[] cs, int pNum, Transform spawnP, GameObject cam)
    {
        playerNum = pNum;
        controllerNum = ctrNum;
        whichCar = carNum;
        cars = cs;
        spawnPoint = spawnP;
        cameraFab = cam;
        initCar();
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        checkCamToggle();
    }
    void checkCamToggle()
    {
        if (controllerNum != -1)
        {
            if (InputPlus.GetData(controllerNum + 1, ControllerVarEnum.ShoulderTop_left) > 0)
            {
                carCam.BroadcastMessage("toggle");
            }
        }
        //carCam.BroadcastMessage("setTarget", (car.transform));
    }
    void initCar()
    {
        car = instantiateCarAndPos(cars[whichCar], playerNum);

        car.BroadcastMessage ("assignPlayerNumber", playerNum);
        car.BroadcastMessage("enableDrive");
		car.BroadcastMessage("setLastCheckpoint", spawnPoint.gameObject);
        //car.BroadcastMessage("setTrack", this.gameObject);
		carCam = Instantiate (cameraFab);
        car.BroadcastMessage("assignCam", carCam);
        carCam.BroadcastMessage("setTarget", (car.transform));
        carCam.transform.SetParent(car.transform);
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
}
