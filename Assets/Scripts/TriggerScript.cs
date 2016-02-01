using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
    }
	// Update is called once per frame
	void Update () {
	
	}
}
