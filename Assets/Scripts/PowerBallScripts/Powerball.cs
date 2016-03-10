using UnityEngine;
using System.Collections;

public class Powerball : MonoBehaviour{
    private PowerballBallBehavior behave;
    public bool active = false;
    public void assignBehavior(PowerballBallBehavior _behave)
    {
        behave = _behave;
    }
    public void act()
    {
        if(behave != null)
        {
            if (active)
            {
                GetComponent<Rigidbody>().useGravity = false;
                behave.activeAct();
            }
            else
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                behave.inactiveAct();
            }
        }
    }
    public void OnTriggerEnter()
    {
        Debug.Log("Trigggggerrr");
    }
}
