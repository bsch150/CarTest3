using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {
    private int number;
    private GameObject track;
	// Use this for initialization
    void assignNumber(int num)
    {
        number = num;
    }
	void Start () {
	
	}
    void assignTrack(GameObject t)
    {
        track = t;
    }
	void OnTriggerEnter(Collider other)
    {
        //other.gameObject.BroadcastMessage("checkCheckpoint", number);
        track.BroadcastMessage("checkCheckpoint", new Vector2(0,number));
    }
	// Update is called once per frame
	void Update () {
	
	}
}
