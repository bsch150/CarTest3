using UnityEngine;
using System.Collections;

public class HubcapScript : MonoBehaviour
{
    public GameObject hb0;
    public GameObject hb1;
    private int which;
    private int numCaps = 2;
    private GameObject cap;
	// Use this for initialization
	void Start () {
        which = PlayerPrefs.GetInt("whichHubcap", 0);
        switch (which)
        {
            case 0:
                cap = Instantiate(hb0);
                break;
            case 1:
                cap = Instantiate(hb1);
                break;
            default:
                cap = Instantiate(hb0);
                break;
        }
        cap.transform.parent = this.transform;
        //this.transform.localPosition = new Vector3(0, 0, 0);
        cap.transform.localPosition = new Vector3(0, 0, 0);
        cap.transform.rotation = new Quaternion(0,0,0,0);
        cap.transform.localScale = new Vector3(1, 1, 1);
    }
    void switchUp()
    {
        switchCap(1);
    }
    void switchDown()
    {
        switchCap(-1);
    }
    void switchCap(int howMuch)
    {
        which += howMuch;
        if (which < 0) which = numCaps - 1;
        if (which >= numCaps) which = 0;
        Destroy(cap);
        switch (which)
        {
            case 0:
                cap = Instantiate(hb0);
                break;
            case 1:
                cap = Instantiate(hb1);
                break;
            default:
                cap = Instantiate(hb0);
                break;
        }
        PlayerPrefs.SetInt("whichHubcap", which);
        cap.transform.parent = this.transform;
        //this.transform.localPosition = new Vector3(0, 0, 0);
        cap.transform.localPosition = new Vector3(0, 0, 0);
        cap.transform.rotation = new Quaternion(0, 0, 0, 0);
        cap.transform.localScale = new Vector3(1, 1, 1);
    } 
	// Update is called once per frame
	void Update () {
	
	}
}
