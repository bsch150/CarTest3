using UnityEngine;
using System.Collections;

public class MainGarage : MonoBehaviour {
    public GameObject LittleRed;
    public GameObject LittleBlue;
	private int whichCar;
    private int numCars = 2;
    private GameObject[] cars;
    private GameObject car;
    private int counter = 0;
    // Use this for initialization
    void Start ()
    {
        cars = new GameObject[numCars];
        cars[0] = LittleRed;
        cars[1] = LittleBlue;
    PlayerPrefs.SetInt ("whichCar", 0);
		whichCar = PlayerPrefs.GetInt ("whichCar");
        car = Instantiate(cars[whichCar]);
	}

    void incrementSelection(int howMuch)
    {
        counter = 100;
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
    void Update() {
        counter--;
        float hAxis = Input.GetAxis("Horizontal");
        if (hAxis != 0) {
            if (counter < 0)
            {
                if (hAxis > 0)
                {
                    incrementSelection(1);
                }
                else if (hAxis < 0)
                {
                    incrementSelection(-1);
                }
            }
            
        }
        else
        {

        }
	}
}
