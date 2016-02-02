using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
    {
        Application.LoadLevel("Garage");
    }
	// Update is called once per frame
	void Update () {
	
	}
}
