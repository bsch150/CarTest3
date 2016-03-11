using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointScript : MonoBehaviour {
    private int number;
    private TrackScript track;
    public GameObject[] fans;
    public GameObject flag;
    private List<Fan> fan;
    float currentSin = 0;
    float bobSpeed = .05f;
	// Use this for initialization
    void assignNumber(int num)
    {
        number = num;
    }
	void Start () {
        fan = new List<Fan>();
        if (fans != null)
        {
            foreach (GameObject g in fans)
            {
                if (g != null)
                {
                    Fan temp = g.AddComponent<Fan>();

                    fan.Add(temp);
                }
            }
        }
	}
    void assignTrack(TrackScript t)
    {
        track = t;
    }
	void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            string playerTag = other.gameObject.GetComponentInParent<Rigidbody>().gameObject.tag;
            if (playerTag.Contains("player"))
            {
                Debug.Log("player collided with check trigger");
                track.checkCheckpoint(number, playerTag);
            }
        }
    }
	// Update is called once per frame
	void Update () {
        if(flag != null)flag.transform.localPosition = flag.transform.localPosition + new Vector3(Mathf.Sin(currentSin)/40,0,0);
        currentSin += bobSpeed;
	}
}
