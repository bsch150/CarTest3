using UnityEngine;
using System.Collections;
using System;

public class PowerBallSpawner : MonoBehaviour {
    public int[] WhatThisSpawns;
    public bool hasBall;
    public GameObject pBallFab;
    private GameObject pBall;
    private Powerball pBallScr;
    private int noBallTimer = 0;
    private int spawnTime = 1000;
	// Use this for initialization
	void Start () {
        Debug.Log("Testing");
        spawn();
	}
    void spawn()
    {
        Debug.Log("Spawning...");
        hasBall = true;
        pBall = Instantiate(pBallFab);
        pBall.transform.SetParent(transform);
        pBallScr = pBall.GetComponent<Powerball>();
        pBallScr.assignBehavior(getRandToSpawn(),Vector3.up * 2,this);
        pBallScr.active = false;
    }
	PowerballBallBehavior getRandToSpawn()
    {
        int which = WhatThisSpawns[Mathf.RoundToInt(UnityEngine.Random.Range(0, 1) * WhatThisSpawns.Length)];
        if(which == 0)
        {
            return new Explosive();
        }
        else
        {
            return new Explosive();
        }
    }
	// Update is called once per frame
	void Update () {
        if (!hasBall)
        {
            noBallTimer++;
            if (noBallTimer > spawnTime)
            {
                spawn();
                noBallTimer = 0;
            }
        }
        else
        {
            pBallScr.act();
        }
	}
}
