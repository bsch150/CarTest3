using UnityEngine;
using System.Collections;
using System;

public class Powerball : MonoBehaviour{
    private PowerballBallBehavior behave;
    public bool active = false;
    private Rigidbody rb;
    public Vector3 startPos;
    private PowerBallSpawner spawner;
    public void assignBehavior(PowerballBallBehavior _behave, Vector3 initPos, PowerBallSpawner sp)
    {
        spawner = sp;
        behave = _behave;
        rb = GetComponent<Rigidbody>();
        startPos = initPos;
    }
    public void act()
    {
        if(behave != null)
        {
            if (active)
            {
                rb.useGravity = true;
                behave.activeAct();
            }
            else
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(0, 0, 0);
                transform.localPosition = startPos;
                behave.inactiveAct();
            }
        }
    }
    public void disownSpawn()
    {
        spawner.hasBall = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!active && !other.isTrigger)
        {
            try {
                if (other.GetComponentInParent<Rigidbody>().gameObject.tag.Contains("player")) {
                    Debug.Log("Player collided");
                    other.GetComponentInParent<CarController>().collideWithPowerball(this);
                }
            }
            catch(NullReferenceException e)
            {
                Debug.Log(e);
            }
            
        }
    }
}
