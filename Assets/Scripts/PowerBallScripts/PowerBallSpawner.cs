using UnityEngine;
using System.Collections;
using System;

public class PowerBallSpawner : MonoBehaviour {
    public int[] WhatThisSpawns;
    private bool hasBall;
    public GameObject pBallFab;
    private GameObject pBall;
    private Powerball pBallScr;
    private int noBallTimer = 0;
    private int spawnTime = 100;
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
        pBall.transform.position = transform.position + Vector3.up;
        pBallScr = pBall.GetComponent<Powerball>();
        pBallScr.assignBehavior(getRandToSpawn());
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
            }
        }
        else
        {
            pBallScr.act();
        }
	}
}
