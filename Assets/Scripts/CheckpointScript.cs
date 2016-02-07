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

		if (other.gameObject.tag == "car") {

			Debug.Log ("chekc collided");
			if(number == 0){
				other.GetComponentInParent<Rigidbody>().gameObject.BroadcastMessage ("checkNewLap", track);
			}else{
				other.GetComponentInParent<Rigidbody>().gameObject.BroadcastMessage("checkCheckpoint",number);
			}
		}
    }
	// Update is called once per frame
	void Update () {
	
	}
}
