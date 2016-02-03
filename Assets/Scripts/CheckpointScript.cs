using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {
    public int number;
    public Transform trackTransform;
	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider other)
    {
        //other.gameObject.BroadcastMessage("checkCheckpoint", number);
        trackTransform.gameObject.BroadcastMessage("checkCheckpoint", other);
    }
	// Update is called once per frame
	void Update () {
	
	}
}
