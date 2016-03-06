using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TrackScript : MonoBehaviour
{
    public string trackName;
    //public int NumberOfCheckPoints;
    public int numLaps;
    public GameObject[] chks;


    private double trackTime;
    private double startTime = -1;
	Vector3[] carProgress;
    float[] startTimes;
    WorldScript world;

    int getPNumFromTag(string tag)
    {
        string temp = tag.Substring(tag.Length - 1, 1);
        //world.log(temp);
        int ret = -1;
        try {
            ret = System.Int32.Parse(tag.Substring(tag.Length - 1, 1));
        }
        catch(System.FormatException e)
        {
            Debug.Log(e);
        }
        return ret;
    }

    void assignWorld(WorldScript _world)
    {
        world = _world;
    }
    public void checkCheckpoint(int checkNum, string objectTag)
    {
        int num = getPNumFromTag(objectTag);
        PlayerController p = world.players[num - 1];
        if (p.currentTrack == "NONE") //player is not in a track
        {
            if(checkNum == 0 && p.finisherTimer > 50)//starting a track
            {
                p.currentTrack = trackName;
                carProgress[p.playerNum - 1] = new Vector3(0, 0,carProgress[p.playerNum - 1][2]);
                update();
            }
        }
        else if(p.currentTrack == trackName) //player is in a track and it's this one
        {
            Vector3 thisProg = carProgress[p.playerNum - 1];
            if(thisProg != null)
            {
                int currLap = System.Convert.ToInt32(thisProg[0]);
                int currCheck = System.Convert.ToInt32(thisProg[1]);
                if(checkNum == 0)
                {
                    if(currCheck == chks.Length - 1)//Validly driving through check 0
                    {
                        if (currLap == numLaps - 1)//Finsh race
                        {
                            carProgress[p.playerNum - 1] = new Vector3(-1,-1, carProgress[p.playerNum - 1][2]);
                            update();
                            p.currentTrack = "NONE";
                            p.finisherTimer = 0;
                        }
                        else//starting new lap
                        {
                            carProgress[p.playerNum - 1] = new Vector3(currLap + 1, 0, carProgress[p.playerNum - 1][2] - startTimes[p.playerNum - 1]);
                            update();
                        }
                    }
                    else
                    {
                        //prompt off Track?
                    }
                }
                else//CheckNum not 0
                {
                    if(currCheck == checkNum - 1)//validly driving through chk
                    {
                        carProgress[p.playerNum - 1] = new Vector2(currLap, currCheck + 1);
                        update();
                    }
                    else{
                        //prompt off course
                    }
                }
            }
            else
            {
                Debug.Log("carProgress at " + (p.playerNum - 1) + " was null, should not have been.");
            }
        }
        else //player is in a track and its not this one
        {
            //prompt p with "off track?"
        }
    }

    void initCheckpoints()
    {
        for(int i = 0; i < chks.Length; i++)
        {
            chks[i].BroadcastMessage("assignTrack", this);
            chks[i].BroadcastMessage("assignNumber", i);
        }
    }
	void checkNewLap(Vector3 info){
	}
    Vector3[] getInitCarProg()
    {
        Vector3[] ret = new Vector3[8];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = new Vector3(-1, -1,-1);
        }
        return ret;
    }
    void Start () {
        initCheckpoints();
        trackTime = 0;
        carProgress = getInitCarProg();
        startTimes = new float[8];
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
	void update () {
        foreach(PlayerController p in world.players)
        {
            p.setUI(carProgress[p.playerNum - 1]);
        }
    }
}
