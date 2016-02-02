using UnityEngine;
using System.Collections;

public class TrackScript : MonoBehaviour
{
    public GameObject LittleRed;
    public GameObject LittleBlue;
    public GameObject carCam;
    public Transform spawnPoint;
    private GameObject car;

    // Use this for initialization
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
        carCam.BroadcastMessage("setTarget", car.transform);
    }
    void Start () {
        initCar();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
