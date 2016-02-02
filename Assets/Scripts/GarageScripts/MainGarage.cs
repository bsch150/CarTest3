﻿using UnityEngine;
using System.Collections;
using System;

public class MainGarage : MonoBehaviour {
    public GameObject LittleRed;
    public GameObject LittleBlue;
	private int whichCar;
    private int whichHubcap;
    private int numCars = 2;
    private GameObject[] cars;
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
        cars = new GameObject[numCars];
        cars[0] = LittleRed;
        cars[1] = LittleBlue;
        setPlayerPrefs();
        whichCar = PlayerPrefs.GetInt("whichCar");
        whichHubcap = PlayerPrefs.GetInt("whichHubcap");
        car = Instantiate(cars[whichCar]);
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
        if (Math.Abs(hAxis) > 0.05f)
        {
            Debug.Log("vAxis = " + vAxis);
            if (counter < 0)
            {
                if (hAxis > 0.05f)
                {
                    incrementSelection(1);
                }
                else if (hAxis < -.05f)
                {
                    incrementSelection(-1);
                }
            }

        }
        else
        {

        }
        if (Math.Abs(vAxis) > 0.05f)
        {
            if (counter < 0)
            {
                counter = 20;
                if (vAxis > 0.05f)
                {
                    Debug.Log("test");
                    car.BroadcastMessage("switchUp");
                }
                else if (vAxis < -.05f)
                {
                    car.BroadcastMessage("switchDown");
                }
            }
        }
    }
    void Update() {
        counter--;
        float exit = Input.GetAxis("EBrake");
        if(exit > 0)
        {
            Application.LoadLevel("Car");
        }
        float hAxis = Input.GetAxis("DPadX");
        float vAxis = Input.GetAxis("DPadY");
        doSwitches(hAxis, vAxis);
	}
}
