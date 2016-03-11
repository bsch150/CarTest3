using UnityEngine;
using System.Collections;

public class PowerballManager : MonoBehaviour{
    public bool equipped = false;
    GameObject pBallFab;
    PlayerController player;
    private int fireCounter = 0;
    Powerball behave;
    public PowerballManager(GameObject _pBallFab, PlayerController _player)
    {
        pBallFab = _pBallFab;
        player = _player;
        behave = null;
    }
    public void checkAdd(PlayerController _player, Powerball _behave)
    {
            player = _player;
            if (!equipped) {
                behave = _behave;
            behave.disownSpawn();
                behave.transform.SetParent(player.carScript.transform);
                behave.transform.rotation = new Quaternion(0, 0, 0, 0);
                behave.transform.position = player.carScript.transform.position  + new Vector3(0,3,0);
                behave.startPos =  new Vector3(0, 3, 0);
            equipped = true;
            }
            else
            {

            }
        
    }
    public void act()
    {
        if(behave != null && equipped)
        {
            //behave.startPos = player.carScript.transform.position + Vector3.up;
            behave.startPos = new Vector3(0, 3, 0);
            behave.act();
        }
    }
    public void fire(Vector3 velocity)
    {
        if (equipped)
        {
            behave.GetComponent<Rigidbody>().velocity = player.carScript.transform.forward * (20) + velocity;
            behave.GetComponent<Rigidbody>().useGravity = true;
            behave.active = true;
            equipped = false;
            behave = null;
        }
    }
}
