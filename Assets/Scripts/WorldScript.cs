using UnityEngine;
using System.Collections;
using InputPlusControl;
using System.Collections.Generic;

public class WorldScript : MonoBehaviour {

    // Use this for initialization

	public GameObject[] cars;
    public Transform spawnPoint;
    public GameObject[] UIs;
	public GameObject cameraFab;
    public GameObject[] tracks; 


    private GameObject tui;
    private int resetCounter = 0;
    private GameObject[] activeCars;
	private GameObject[] cams;
	private int actualActive;
	private int numControllers;
    private int[] controllerNumToPlayerNum;
    private int[] playerNumToControllerNum;
    private int playerNumCounter = 0;
    private GameObject currentUI;
    private float gravityStrength = -25f;
    private List<PlayerController> players;

    public GameObject getCar(int wc)
    {
        return cars[wc];
    }

    void initTracks()
    {
        for(int i = 0; i < tracks.Length; i++)
        {
			//tracks[i].BroadcastMessage("assignActiveCars", activeCars);
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
        //temp.BroadcastMessage("freeze");
		temp.transform.position = spawnPoint.transform.position + new Vector3 (-6 + (num * 4), 0, 0);
		temp.transform.rotation = spawnPoint.transform.rotation;
		return temp;
		
	}
    void fillControllerPlayerArray()
    {
        controllerNumToPlayerNum = new int[8];
        playerNumToControllerNum = new int[8];
        for (int j = 0; j < controllerNumToPlayerNum.Length; j++) controllerNumToPlayerNum[j] = -1;
        for (int j = 0; j < playerNumToControllerNum.Length; j++) playerNumToControllerNum[j] = -1;
        int i = 0;
        while (PlayerPrefs.GetInt("p" + i, -1) != -1)
        {
            Debug.Log("trying to fillArray at i = " + i);
            int ctrNum = PlayerPrefs.GetInt("p"+i, -1);
            PlayerPrefs.SetInt("p" + i, -1);
            controllerNumToPlayerNum[ctrNum] = i;
            playerNumToControllerNum[i] = ctrNum;
            i++;
            playerNumCounter++;
        }
    }
    void checkControllers()
    {
        numControllers = PlayerPrefs.GetInt("numControllers", -1);
        var temp = Input.GetJoystickNames();
        foreach (string s in temp)
        {
            Debug.Log("s = " + s);
        }
        if (numControllers == -1 || temp.Length != numControllers)
        {
            numControllers = temp.Length;
            PlayerPrefs.SetInt("numControllers", numControllers);
        }
    }
    void Start ()
    {
        players = new List<PlayerController>();
        setGravity(gravityStrength);
        fillControllerPlayerArray();
        checkControllers();
        var toAdd = 1;//PlayerPrefs.GetInt ("actualActive", 1);
		cams = new GameObject[toAdd];
		for (int i = 0; i < toAdd; i++) {
			addPlayer ();
		}
        currentUI = Instantiate(UIs[toAdd - 1]);
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
            players.Add(new PlayerController(playerNumToControllerNum[actualActive],wc,cars,actualActive+1,spawnPoint,cameraFab));
			//initCar (actualActive);
			initTracks ();
			actualActive++;
            PlayerPrefs.SetInt("actualActive", actualActive);
			if(actualActive > 1){
			splitCams();
			}
		}
	}
	void FixedUpdate () {
        foreach(PlayerController p in players)
        {
            p.updateThis();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("Menu");
        }
		for (int i = 0; i < numControllers; i++)
        {
            if (controllerNumToPlayerNum[i] == -1) //controller not registered to car yet
                if (InputPlus.GetData(i+ 1, ControllerVarEnum.Interface_right) > 0) //if controller i is pressing start
            {
                    if (playerNumCounter < actualActive) //If the players all have controller assigned to them we need to create a new player
                    {
                        Debug.Log("Controller " + (i + 1) + " pushed start");
                        if (playerNumCounter < 2)//temporary becasue we don't handle more than 2 right now.
                        {
                            controllerNumToPlayerNum[i] = playerNumCounter;
                            playerNumToControllerNum[playerNumCounter] = i;
                            PlayerPrefs.SetInt("p" + playerNumCounter, i);
                            playerNumCounter++;
                            //TODO assigncontroller to car and handle that in the car's calls.
                            //activeCars[playerNumCounter - 1].BroadcastMessage("assignControllerNumber", i);
                            players[playerNumCounter - 1].setControllerNum(i);

                        }
                    }
                    else
                    {
                        Debug.Log("ctr " + i + " is trying to add player");

                        addPlayer();

                    }
            }else if (controllerNumToPlayerNum[i] != -1 &&  controllerNumToPlayerNum[i] < actualActive)
                {
                    float resetPushed = InputPlus.GetData(playerNumToControllerNum[i] + 1, ControllerVarEnum.FP_top);//Input.GetAxis ("Reset"+(i+1).ToString ());
                    if (resetPushed > 0)
                    {
                        resetCounter++;
                        if (resetCounter == 60)
                        {
                            activeCars[i].BroadcastMessage("reset");
                        }
                    }
                    else {
                        resetCounter = 0;
                    }
                
			}else
                {

                    var temp = InputPlus.GetData(i+1, ControllerVarEnum.Interface_right);//("Start"+(i+1).ToString ());
				if(temp > 0){
				}
			}
		}
    }
    void setGravity(float strength)
    {
        Physics.gravity = new Vector3(0, strength, 0);
    }
}
