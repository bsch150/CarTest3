using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {
    private int lifeSpan;
    private int maxLife = 200;
    private float explosiveForce = 200f;
    private int deadCount = -1;
    public GameObject exp;
	// Use this for initialization
	void Start () {
        lifeSpan = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log("test");
        deadCount--;
        lifeSpan++;
        if (lifeSpan == maxLife)
        {
            Explode();
        }
        if (deadCount == 0){
            Destroy(exp);
        }

	}
    void OnTriggerEnter(Collider other)
    {
        if (lifeSpan > 50)
        {
            Explode(other);
            Debug.Log("OnTrigger");
        }
    }
    void Explode()
    {
        var ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        deadCount = 70;
    }
    void Explode(Collider other)
    {
        other.attachedRigidbody.AddForceAtPosition((other.transform.position - transform.position) * 500000f,new Vector3(0,0,0));
        Explode();
    }
}
