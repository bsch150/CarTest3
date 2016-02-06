using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrackScript : MonoBehaviour
{
    public int NumberOfCheckPoints;
    public int numLaps;
    public GameObject[] chks;


    private double trackTime;
    private double startTime = -1;
    private GameObject car;
    

    void assignCar(GameObject t)
    {
        car = t;
    }
    //This sends a message to the car that passed through a checkpoint. 
    //The vector is just so I can get two parameters into a braodcastMessage. [0] is the collider (car) and [1] is which checkPoint
    void checkCheckpoint(Vector2 num)
    {
        car.BroadcastMessage("assignNumLaps", numLaps);
        car.BroadcastMessage("assignNumChks", chks.Length);
        if (num[1] == 0)
        {
            startTime = Time.time;
        }
        car.BroadcastMessage("setLastCheckpoint",chks[(int)num[1]]);
        car.BroadcastMessage("checkCheckpoint", num[1]);
    }

    void initCheckpoints()
    {
        for(int i = 0; i < chks.Length; i++)
        {
            chks[i].BroadcastMessage("assignTrack", this.gameObject);
            chks[i].BroadcastMessage("assignNumber", i);
        }
    }

    void Start () {
        initCheckpoints();
        trackTime = 0;
    }
	void updateTime()
    {
        trackTime = Time.time - startTime;
    }
	// Update is called once per frame
    void finishTrack()
    {
        startTime = -1;
    }
	void Update () {
        if (startTime != -1)//starTime == -1 means you aren't in a race.
        {
            updateTime();

            car.BroadcastMessage("setTimeText",trackTime);
        }
    }
}
