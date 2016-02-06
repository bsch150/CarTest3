using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour {

    // Use this for initialization

	public GameObject[] cars;
    public Transform spawnPoint;
    public GameObject UI;
    public GameObject[] tracks; 


    private GameObject tui;
    private int resetCounter = 0;
    private GameObject[] activeCars;
	private int actualActive;
	private int numControllers;


    void initCar(int carNum)
    {
		int whichCar = PlayerPrefs.GetInt ("whichCar" + carNum, 0);
		Debug.Log ("carNum = " + carNum);
		activeCars[carNum] = Instantiate (cars [whichCar]);
		
		activeCars[carNum].BroadcastMessage ("assignPlayerNumber", carNum + 1);
		activeCars[carNum].BroadcastMessage("enableDrive");
		activeCars[carNum].transform.position = spawnPoint.position;
		activeCars[carNum].transform.rotation = spawnPoint.rotation;
		activeCars[carNum].BroadcastMessage("setLastCheckpoint", spawnPoint.gameObject);
		activeCars[carNum].BroadcastMessage("setTrack", this.transform);
        tui = Instantiate(UI);
		activeCars[carNum].BroadcastMessage("setUI", tui);


    }

    void initTracks()
    {
        for(int i = 0; i < tracks.Length; i++)
        {
			tracks[i].BroadcastMessage("assignActiveCars", activeCars);
        }
    }

    void Start ()
    {
		var temp = Input.GetJoystickNames ();
		numControllers = temp.Length;
		for (int i = 0; i < temp.Length; i++) {
			Debug.Log ("temp[i] = " + temp[i]);
		}
		activeCars = new GameObject[actualActive+1];
        initCar(0);
		actualActive = 1;
        initTracks();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		for (int i = 0; i < numControllers; i++) {
			if(i < actualActive){
				float resetPushed = Input.GetAxis ("Reset"+(i+1).ToString ());
				if (resetPushed > 0) {
					resetCounter++;
					if (resetCounter == 60) {
						activeCars [0].BroadcastMessage ("reset");
					}
				} else {
					resetCounter = 0;
				}
			}else {
				var temp = Input.GetAxis("Start"+(i+1).ToString ());
				Debug.Log ("Start input = " + temp);
				if(temp > 0){
					int wc = PlayerPrefs.GetInt("whichCar"+(actualActive+1).ToString (),0);
					GameObject[] tempForCopy = new GameObject[actualActive + 1];
					for(int j = 0; j < actualActive; j++){
						tempForCopy[j] = activeCars[j];
					}
					tempForCopy[actualActive] = Instantiate(cars[wc]);
					activeCars = tempForCopy;
					initTracks ();
					initCar (actualActive);
					actualActive++;
					for(int j = 0; j < actualActive;j++){
						activeCars[j].BroadcastMessage("splitCam",actualActive);
					}

				}
			}
		}
    }
}
