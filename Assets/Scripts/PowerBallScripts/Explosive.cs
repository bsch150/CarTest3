using UnityEngine;
using System.Collections;

public class Explosive : PowerballBallBehavior {
    private int lifeSpan;
    private int maxLife = 200;
    private float explosiveForce = 200f;
    private int deadCount = -1;
    public GameObject exp;
    // Use this for initialization
    override public void collideWith(Collider other, Powerball ball)
    {
        if(lifeSpan > 50)
        {
            Explode(other,ball);
            Debug.Log("OnTrigger");
        }
    }
    override public void activeAct()
    {
        lifeSpan++;
    }
    override public void inactiveAct()
    {

    }
    void Explode()
    {
        //var ps = GetComponentInChildren<ParticleSystem>();
        //ps.Play();
        deadCount = 70;
    }
    void Explode(Collider other,Powerball ball)
    {
        var temp = other.GetComponentInParent<Rigidbody>();
        if (temp != null)
        {
            temp.AddForceAtPosition((other.transform.position - ball.transform.position) * 500000f, new Vector3(0, 0, 0));
        }
        Explode();
    }
}
