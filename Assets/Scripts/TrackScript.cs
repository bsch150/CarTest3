using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrackScript : MonoBehaviour
{
    public GameObject LittleRed;
    public GameObject LittleBlue;
    public GameObject carCam;
    public Transform spawnPoint;
    public int NumberOfCheckPoints;
    public GameObject chk0;
    public GameObject chk1;
    public GameObject chk2;
    public GameObject chk3;
    public GameObject chk4;
    public GameObject chk5;
    public GameObject chk6;
    public GameObject chk7;
    private GameObject[] chks;
    private GameObject car;
    public GameObject UI;
    private GameObject tui;
    private double trackTime;
    private double startTime = -1;

    GameObject getChkFromInt(int num)
    {
        switch (num)
        {
            case 0:
                return chk0;
            case 1:
                return chk1;
            case 2:
                return chk2;
            case 3:
                return chk3;
            case 4:
                return chk4;
            case 5:
                return chk5;
            case 6:
                return chk6;
            case 7:
                return chk7;
            default:
                return null;
        }
    }

    void fillArray()
    {
        chks = new GameObject[NumberOfCheckPoints];
        for(int i = 0; i < NumberOfCheckPoints; i++)
        {
            chks[i] = getChkFromInt(i);
        }
    }
    //This sends a message to the car that passed through a checkpoint. 
    //The vector is just so I can get two parameters into a braodcastMessage. [0] is the collider (car) and [1] is which checkPoint
    void checkCheckpoint(Vector2 num)
    {
        if(num[1] == 0)
        {
            startTime = Time.time;
        }
        car.BroadcastMessage("checkCheckpoint", num[1]);
    }
    void initCar()
    {
        int whichCar = PlayerPrefs.GetInt("whichCar", 0);
        switch (whichCar)
        {
            case 0:
                car = Instantiate(LittleRed);
                break;
            case 1:
                car = Instantiate(LittleBlue);
                break;

        }

        car.BroadcastMessage("enableDrive");
        car.transform.position = spawnPoint.position;
        car.transform.rotation = spawnPoint.rotation;
        car.BroadcastMessage("setTrack", this.transform);
        tui = Instantiate(UI);
        car.BroadcastMessage("setUI", tui);
        car.BroadcastMessage("setNumChks", NumberOfCheckPoints);
        carCam.BroadcastMessage("setTarget", car.transform);
        

    }
    void Start () {
        fillArray();
        initCar();
        trackTime = 0;
    }
	void updateTime()
    {
        trackTime = Time.time - startTime;
    }
	// Update is called once per frame
	void Update () {
        if (startTime != -1)//starTime == -1 means you aren't in a race.
        {
            updateTime();

            car.BroadcastMessage("setTimeText",trackTime);
        }
	}
}
