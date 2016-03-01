using UnityEngine;
using System.Collections;

public class GarageScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    void OnTriggerEnter(Collider other)
    {
        other.gameObject.BroadcastMessage("enterGarage");
    }

    void OnTriggerExit(Collider other)
    {
        other.gameObject.BroadcastMessage("exitGarage");
    }
    GameObject instantiateCarAndPos(GameObject c, int num)
    {
        GameObject temp = Instantiate(c);
        temp.transform.position = temp.transform.position + new Vector3(-6 + (num * 4), 0, 0);
        return temp;
    }

}
