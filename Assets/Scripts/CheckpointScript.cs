using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {
    private int number;
    private TrackScript track;
	// Use this for initialization
    void assignNumber(int num)
    {
        number = num;
    }
	void Start () {
	
	}
    void assignTrack(TrackScript t)
    {
        track = t;
    }
	void OnTriggerEnter(Collider other)
    {
        string playerTag = other.gameObject.GetComponentInParent<Rigidbody>().gameObject.tag;
        if (playerTag.Contains("player"))
        {
            Debug.Log("player collided with check trigger");
            track.checkCheckpoint(number, playerTag);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
