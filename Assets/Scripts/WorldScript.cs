using UnityEngine;
using System.Collections;

public class WorldScript : MonoBehaviour {

    // Use this for initialization

	public GameObject[] cars;
    public Transform spawnPoint;
    public GameObject UI;
	public GameObject cameraFab;
    public GameObject[] tracks; 


    private GameObject tui;
    private int resetCounter = 0;
    private GameObject[] activeCars;
	private GameObject[] cams;
	private int actualActive;
	private int numControllers;


    void initCar(int carNum)
    {
		int whichCar = PlayerPrefs.GetInt ("whichCar" + (carNum+1).ToString (), 0);
		Debug.Log ("carNum = " + carNum);
		activeCars[carNum] = instantiateCarAndPos (cars [whichCar],carNum);
		
		activeCars[carNum].BroadcastMessage ("assignPlayerNumber", carNum + 1);
		activeCars[carNum].BroadcastMessage("enableDrive");
		activeCars[carNum].BroadcastMessage("setLastCheckpoint", spawnPoint.gameObject);
		activeCars[carNum].BroadcastMessage("setTrack", this.gameObject);
        tui = Instantiate(UI);
		activeCars[carNum].BroadcastMessage("setUI", tui);
		cams [carNum] = Instantiate (cameraFab);
		cams [carNum].BroadcastMessage("setTarget", (activeCars [carNum].transform));
		cams[carNum].BroadcastMessage ("assignPlayerNum", carNum+1);

    }

    void initTracks()
    {
        for(int i = 0; i < tracks.Length; i++)
        {
			tracks[i].BroadcastMessage("assignActiveCars", activeCars);
        }
    }
	
	void splitCams(){
		for(int i = 0; i < cams.Length;i++){
			Camera c = cams[i].GetComponentInChildren<Camera> ();
			cams [i].BroadcastMessage("setTarget", (activeCars [i].transform));
			//Debug.Log ("set "+i+" to " + activeCars [i].transform);
			if (i == 0) {

				Debug.Log("was 0");

				c.rect = new Rect (0f, .5f, 1f, .5f);
			} else if (i == 1) {
				Debug.Log("was 1");
				c.rect = new Rect (0f, 0f, 1f, .5f);
			}
		}
	}
	GameObject instantiateCarAndPos(GameObject c, int num){
		GameObject temp = Instantiate (c);
		temp.transform.position = spawnPoint.transform.position + new Vector3 (-6 + (num * 4), 0, 0);
		temp.transform.rotation = spawnPoint.transform.rotation;
		return temp;
		
	}
    void Start ()
	{
		numControllers = PlayerPrefs.GetInt ("numControllers", -1);
		var temp = Input.GetJoystickNames ();
		if (numControllers == -1 || temp.Length != numControllers) {
			numControllers = temp.Length;
			PlayerPrefs.SetInt ("numControllers",numControllers);
		}
		var toAdd = PlayerPrefs.GetInt ("actualActive", 1);
		cams = new GameObject[toAdd];
		//Debug.Log ("toAdd = " + toAdd);
		for (int i = 0; i < temp.Length; i++) {
			//Debug.Log ("temp[i] = " + temp[i]);
		}
		for (int i = 0; i < toAdd; i++) {
			addPlayer ();
		}
		
        initTracks();
    }
	
	// Update is called once per frame
	void addPlayer(){
		if (actualActive < numControllers) {
			int wc = PlayerPrefs.GetInt ("whichCar" + (actualActive + 1).ToString (), 0);
			GameObject[] tempForCopy = new GameObject[actualActive + 1];
			for (int j = 0; j < actualActive; j++) {
				tempForCopy [j] = activeCars [j];
			}
			activeCars = tempForCopy;
			GameObject[] tempCamCopy = new GameObject[actualActive + 1];
			for (int j = 0; j < actualActive; j++) {
				tempCamCopy [j] = cams [j];
			}
			cams = tempCamCopy;
			initCar (actualActive);
			initTracks ();
			actualActive++;
			if(actualActive > 1){
			splitCams();
			}
		}
	}
	void FixedUpdate () {
		//Debug.Log ("numnControllers = " + numControllers);
		
		for (int i = 0; i < activeCars.Length; i++) {
			cams [i].BroadcastMessage ("setTarget", (activeCars [i].transform));
		}
		for (int i = 0; i < numControllers; i++) {
			if(i < actualActive){
				float resetPushed = Input.GetAxis ("Reset"+(i+1).ToString ());
				if (resetPushed > 0) {
					resetCounter++;
					if (resetCounter == 60) {
						activeCars [i].BroadcastMessage ("reset");
					}
				} else {
					resetCounter = 0;
				}
			}else {
				var temp = Input.GetAxis("Start"+(i+1).ToString ());
				//Debug.Log ("Start input = " + temp);
				if(temp > 0){
					addPlayer ();
				}
			}
		}
    }
}
