using UnityEngine;
using System.Collections;
using System;

public class MainGarage : MonoBehaviour {
	public GameObject[] cars;
    private int whichCar;
    private int whichHubcap;
    private int numCars = 4;
    private GameObject car;
    private int counter = 0;
    // Use this for initialization
    void setPlayerPrefs()
    {
        int temp = PlayerPrefs.GetInt("whichCar",-1);
        if (temp == -1) PlayerPrefs.SetInt("whichCar", 0);
         temp = PlayerPrefs.GetInt("whichHubcap", -1);
        if (temp == -1) PlayerPrefs.SetInt("whichHubcap", 0);
    }
    void Start ()
    {
        setPlayerPrefs();
        whichCar = PlayerPrefs.GetInt("whichCar");
        whichHubcap = PlayerPrefs.GetInt("whichHubcap");
        car = Instantiate(cars[whichCar]);
		car.BroadcastMessage ("assignPlayerNumber", 1);
	}

    void incrementSelection(int howMuch)
    {
        counter = 20;
        int temp = howMuch;
        whichCar += howMuch;
        if(whichCar >= numCars)
        {
            whichCar = 0;
        }else if(whichCar < 0)
        {
            whichCar = numCars - 1;
        }
        Destroy(car);
        car = Instantiate(cars[whichCar]);
        PlayerPrefs.SetInt("whichCar", whichCar);
    }
    // Update is called once per frame
    void doSwitches(float hAxis, float vAxis)
    {
        if (Math.Abs(hAxis) > 0.5f)
        {
            Debug.Log("vAxis = " + vAxis);
            if (counter < 0)
            {
                if (hAxis > 0.5f)
                {
                    incrementSelection(1);
                }
                else if (hAxis < -.5f)
                {
                    incrementSelection(-1);
                }
            }

        }
        else
        {

        }
        if (Math.Abs(vAxis) > 0.5f)
        {
            if (counter < 0)
            {
                counter = 20;
                if (vAxis > 0.5f)
                {
                    Debug.Log("test");
                    car.BroadcastMessage("switchUp");
                }
                else if (vAxis < -.5f)
                {
                    car.BroadcastMessage("switchDown");
                }
            }
        }
    }
    void Update() {
        counter--;
        float exit = Input.GetAxis("SelectMenu1");
        //Debug.Log("exit = " + exit);
        if(exit > 0)
        {
            Application.LoadLevel("Car");
        }
        float hAxis = Input.GetAxis("Horizontal1");
        float vAxis = Input.GetAxis("Vertical1");
        doSwitches(hAxis, vAxis);
	}
}
