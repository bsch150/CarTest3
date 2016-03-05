using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
    GameObject ui;
    private Text posText;
	// Use this for initialization
	public UIScript (GameObject uiFab) {
        ui = Instantiate(uiFab);
        Transform[] temp = ui.GetComponentsInChildren<Transform>();
        foreach(Transform t in temp)
        {
            if(t.gameObject.name == "Position")
            {
                posText = t.gameObject.GetComponent<Text>();
            }
        }
	}
	public void setInfo(Vector2 info)
    {
        if(info[0] == -1 || info[1] == -1)
        {
            posText.text = "";
        }
        else
        {
            posText.text = info[0] + ", " + info[1];
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
