using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour {
    private float fanSpeed = .055f;
   	// Use this for initialization
	void Start () {
        transform.rotation = new Quaternion(0, 0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 axis = Vector3.up;
        transform.RotateAroundLocal(Vector3.up, fanSpeed);
	}
}
