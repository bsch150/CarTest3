using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
    GameObject ui;
    private Text posText;
    private Text timeText;
    private Text hsText;
    public Text centerText;
    public Text centerTextSelected;
    private float alpha = .1f;
    private Color currColor;
    public Color destColor;
    public float toggleTimer = Time.time;
    StartScript startScript;
    public int controllerNum;
    // Use this for initialization
    public UIScript (GameObject uiFab, int _controllerNum, PlayerController p) {
        controllerNum = _controllerNum;
        startScript = new StartScript(this, p);
        currColor = new Color(0, 0, 0, alpha);
        ui = Instantiate(uiFab);
        Transform[] temp = ui.GetComponentsInChildren<Transform>();
        foreach(Transform t in temp)
        {
            if (t.gameObject.name == "Position")
            {
                posText = t.gameObject.GetComponent<Text>();
            }
            else if (t.gameObject.name == "Time")
            {
                timeText = t.gameObject.GetComponent<Text>();
            }
            else if (t.gameObject.name == "HighScore")
            {
                hsText = t.gameObject.GetComponent<Text>();
            }
            else if (t.gameObject.name == "Center")
            {
                centerText = t.gameObject.GetComponent<Text>();
            }else if(t.gameObject.name == "CenterSelected")
            {
                centerTextSelected = t.gameObject.GetComponent<Text>();
            }
        }
	}
	public void setInfo(Vector3 info)
    {
        if(info[0] == -1 || info[1] == -1)
        {
            posText.text = "";
            
        }
        else
        {
            posText.text = info[0] + ", " + info[1];
            timeText.text = info[2].ToString(); 
        }
    }
    public void setTint(Color c)
    {
        destColor = new Color(c.r,c.g,c.b,alpha);
    }
	// Update is called once per frame
	public void update () {
        currColor = Color.Lerp(currColor, destColor, .01f);
        ui.GetComponent<Image>().color = currColor;
        if(Time.time - toggleTimer > .2)
        {
            startScript.update();
        }
    }
    public void setHighScoreText(string str)
    {
        hsText.text = str;
    }
    public void toggleStart()
    {
        Debug.Log("ToggleStat called");
        if (Time.time - toggleTimer > 1)
        {
            Debug.Log("time was good");
            toggleTimer = Time.time;
            startScript.show();
        }
        else
        {
            Debug.Log("time was "+(Time.time - toggleTimer));
        }
    }
}
