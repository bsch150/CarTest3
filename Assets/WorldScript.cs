using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour {

    // Use this for initialization

    public GameObject LittleRed;
    public GameObject LittleBlue;
    public GameObject Comanche;
    public GameObject Ettore;
    public GameObject carCam;
    public Transform spawnPoint;
    public GameObject UI;
    public int NumberOfTracks;
    public GameObject track1;
    public GameObject track2;
    public GameObject[] tracks; 


    private GameObject tui;
    private int resetCounter = 0;
    private GameObject car;


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
            case 2:
                car = Instantiate(Comanche);
                break;
            case 3:
                car = Instantiate(Ettore);
                break;

        }

        car.BroadcastMessage("enableDrive");
        car.transform.position = spawnPoint.position;
        car.transform.rotation = spawnPoint.rotation;
        car.BroadcastMessage("setLastCheckpoint", spawnPoint.gameObject);
        car.BroadcastMessage("setTrack", this.transform);
        tui = Instantiate(UI);
        car.BroadcastMessage("setUI", tui);
        carCam.BroadcastMessage("setTarget", car.transform);


    }

    void initTracks()
    {
        for(int i = 0; i < tracks.Length; i++)
        {
            tracks[i].BroadcastMessage("assignCar", car);
        }
    }

    void Start ()
    {
        initCar();
        initTracks();

    }
	
	// Update is called once per frame
	void Update () {

        float resetPushed = Input.GetAxis("Reset");
        if (resetPushed > 0)
        {
            resetCounter++;
            if (resetCounter == 60)
            {
                car.BroadcastMessage("reset");
            }
        }
        else
        {
            resetCounter = 0;
        }
    }
}
