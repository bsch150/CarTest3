using UnityEngine;
using System.Collections;

public class GarageScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }
    bool isPlayer(Collider other)
    {
        if(other.isTrigger)
        {
            return false;
        }
        CarController temp = other.gameObject.GetComponentInParent<CarController>();
        if(temp != null)
        {
            if (temp.tag.Contains("player")) return true;
        }
        return false;
    }
    void OnTriggerEnter(Collider other)
    {
        if(isPlayer(other)) other.gameObject.GetComponentInParent<CarController>().BroadcastMessage("enterGarage");
    }

    void OnTriggerExit(Collider other)
    {
        if (isPlayer(other)) other.gameObject.GetComponentInParent<CarController>().BroadcastMessage("exitGarage");
    }
    GameObject instantiateCarAndPos(GameObject c, int num)
    {
        GameObject temp = Instantiate(c);
        temp.transform.position = temp.transform.position + new Vector3(-6 + (num * 4), 0, 0);
        return temp;
    }

}
