using UnityEngine;
using System.Collections;
using System;
using InputPlusControl;

public class MainGarage : MonoBehaviour {
	public GameObject[] cars;
    private int[] whichCar;
    private int[] whichHubcap;
    private GameObject[] activeCars;
    private int[] counter;
	private int numControllers;
    // Use this for initialization
    void setPlayerPrefs(int num)
    {
        int temp = PlayerPrefs.GetInt("whichCar"+num,-1);
		if (temp == -1) PlayerPrefs.SetInt("whichCar"+num, 0);
		temp = PlayerPrefs.GetInt("whichHubcap"+num, -1);
		if (temp == -1) PlayerPrefs.SetInt("whichHubcap"+num, 0);
    }
    void Start ()
    {
		numControllers = PlayerPrefs.GetInt ("numControllers", -1);
		var temp = Input.GetJoystickNames ();
		if (numControllers == -1 || temp.Length != numControllers) {
			numControllers = temp.Length;
			PlayerPrefs.SetInt ("numControllers",numControllers);
		}
		int num = PlayerPrefs.GetInt("actualActive",1);
		//PlayerPrefs.SetInt ("actualActive", 1);
		counter = new int[num];
		whichCar = new int[num];
		whichHubcap = new int[num];
		activeCars = new GameObject[num];
		for (int j = 0; j < num; j++) {
            Debug.Log("j = " + j);
			int i = j+1;
			setPlayerPrefs (i);
			whichCar[j] = PlayerPrefs.GetInt ("whichCar"+i);
			whichHubcap[j] = PlayerPrefs.GetInt ("whichHubcap"+i);
			activeCars[j] = instantiateCarAndPos (cars [whichCar[j]],j);
			activeCars[j].BroadcastMessage ("assignPlayerNumber", i+1);
		}
	}
	GameObject instantiateCarAndPos(GameObject c, int num){
		GameObject temp = Instantiate (c);
		temp.transform.position = temp.transform.position + new Vector3 (-6 + (num * 4), 0, 0);
		return temp;

	}
    void incrementSelection(int carNum, int howMuch)
    {
        counter[carNum] = 20;
        int temp = howMuch;
		whichCar[carNum] += howMuch;
		if(whichCar[carNum] >= cars.Length)
        {
			whichCar[carNum] = 0;
        }else if(whichCar[carNum] < 0)
        {
			whichCar[carNum] = cars.Length - 1;
        }
        Destroy(activeCars[carNum]);
		activeCars[carNum] = instantiateCarAndPos(cars[whichCar[carNum]],carNum);
        PlayerPrefs.SetInt("whichCar"+(carNum+1).ToString (), whichCar[carNum]);
    }
    // Update is called once per frame
    void doSwitches(int carNum, float hAxis, float vAxis)
    {
        if (Math.Abs(hAxis) > 0.5f)
        {
            Debug.Log("vAxis = " + vAxis);
			if (counter[carNum] < 0)
            {
                if (hAxis > 0.5f)
                {
                    incrementSelection(carNum,1);
                }
                else if (hAxis < -.5f)
                {
					incrementSelection(carNum,-1);
                }
            }

        }
        else
        {

        }
        if (Math.Abs(vAxis) > 0.5f)
        {
            if (counter[carNum] < 0)
            {
				counter[carNum] = 20;
                if (vAxis > 0.5f)
                {
                    Debug.Log("test");
					activeCars[carNum].BroadcastMessage("switchUp");
                }
                else if (vAxis < -.5f)
                {
					activeCars[carNum].BroadcastMessage("switchDown");
                }
            }
        }
    }
	

    void Update() {
		for(int i = 0; i < activeCars.Length;i++){
            int ctrnum = PlayerPrefs.GetInt("p" + (i), -1) + 1;
            Debug.Log("p" + i + " has controller " + ctrnum);
			int j = i +1; //because Input is 1 indexed
      	  counter[i]--;
            float exit = InputPlus.GetData(ctrnum, ControllerVarEnum.FP_bottom);//Input.GetAxis("SelectMenu"+j);
            //Debug.Log("exit = " + exit);
            if (exit > 0)
	        	{
            	Application.LoadLevel("Car");
        	}
            float hAxis = InputPlus.GetData(ctrnum, ControllerVarEnum.ThumbLeft_x);// Input.GetAxis("Horizontal"+j);
        	float vAxis = InputPlus.GetData(ctrnum, ControllerVarEnum.ThumbLeft_y); // Input.GetAxis("Vertical"+j);
        	doSwitches(i,hAxis, vAxis);
		}
		for (int i = 0; i < numControllers; i++) {
			if(i >= activeCars.Length){
				var temp = InputPlus.GetData(i + 1, ControllerVarEnum.Interface_right);//Input.GetAxis ("Start" + (i + 1).ToString ());
                                                                                   //Debug.Log ("Start input = " + temp);
                if (temp > 0) {
					int wc = PlayerPrefs.GetInt ("whichCar" + (i + 1).ToString (), 0);
					GameObject[] tempForCopy = new GameObject[i + 1];
					for (int j = 0; j < activeCars.Length; j++) {
						tempForCopy [j] = activeCars [j];
					}
					tempForCopy [activeCars.Length] = instantiateCarAndPos (cars [wc],activeCars.Length);
					activeCars = tempForCopy;
					PlayerPrefs.SetInt ("actualActive",activeCars.Length);
				}
			}
		}

	}
}
