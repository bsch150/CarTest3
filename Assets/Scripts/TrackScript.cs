using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour
{
    public GameObject LittleRed;
    public GameObject LittleBlue;
    public GameObject carCam;
    public Transform spawnPoint;
    public int NumberOfCheckPoints;
    public GameObject chk0;
    private GameObject car;
    public GameObject UI;

    // Use this for initialization
    void checkCheckpoint(Collider num)
    {
        car.BroadcastMessage("checkCheckpoint", 1);
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
        car.BroadcastMessage("setUI", Instantiate(UI));
        carCam.BroadcastMessage("setTarget", car.transform);
        

    }
    void Start () {
        initCar();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
