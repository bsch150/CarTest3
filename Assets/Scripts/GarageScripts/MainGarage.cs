using UnityEngine;
using System.Collections;

public class MainGarage : MonoBehaviour {
    public GameObject LittleRed;
    public GameObject LittleBlue;
	private int whichCar;
    // Use this for initialization
    void Start () {
		PlayerPrefs.SetInt ("whichCar", 0);
		whichCar = PlayerPrefs.GetInt ("whichCar");
		if (whichCar == 0) {
			Instantiate (LittleRed);
		} else if (whichCar == 1) {
			Instantiate (LittleBlue);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
