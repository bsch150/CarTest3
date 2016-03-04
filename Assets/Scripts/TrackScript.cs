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
	Vector2[] carProgress;
    
    void checkCheckpoint(Vector3 num)
    {
		carProgress [(int)(num [0] - 1)] = new Vector2 (num [1],num [2]);
        //car.BroadcastMessage("assignNumLaps", numLaps);
        //car.BroadcastMessage("assignNumChks", chks.Length);
        if (num[2] == 0)
        {
            startTime = Time.time;
        }
        activeCars[(int)num[0] - 1].BroadcastMessage("setLastCheckpoint",chks[(int)num[2]]);
        //car.BroadcastMessage("checkCheckpoint", num[1]);
    }

    void initCheckpoints()
    {
        for(int i = 0; i < chks.Length; i++)
        {
            chks[i].BroadcastMessage("assignTrack", this.gameObject);
            chks[i].BroadcastMessage("assignNumber", i);
        }
    }
	void checkNewLap(Vector3 info){
		if (info [2] == NumberOfCheckPoints - 1) {
			activeCars[(int)(info[0] - 1)].BroadcastMessage("confirmNewLap");
			if (info [1] == numLaps - 1) {
				activeCars[(int)(info[0] - 1)].BroadcastMessage("finishTrack");
			}
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
			for(int i = 0 ; i < activeCars.Length; i++){
            	activeCars[i].BroadcastMessage("setTimeText",trackTime);
			}
        }
    }
}
