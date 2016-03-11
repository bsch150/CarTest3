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
    private int playerNum;
    private int numPlayers;
    // Use this for initialization
    public UIScript (GameObject uiFab, int _controllerNum, PlayerController p,int _numPlayers) {
        playerNum = p.playerNum;
        numPlayers = _numPlayers;
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
    void splitByNumPlayers()
    {
        int numRows = (numPlayers == 2) ? 2 : (numPlayers == 1) ? 1 : Mathf.RoundToInt(numPlayers / 2);
        float y = (Screen.height / 2) - ((playerNum * ((Screen.height / numRows))));
        posText.transform.localPosition = new Vector3(posText.transform.localPosition[0], y+75, posText.transform.localPosition[2]);
        timeText.transform.localPosition = new Vector3(timeText.transform.localPosition[0], y+400, timeText.transform.localPosition[2]);
        hsText.transform.localPosition = new Vector3(hsText.transform.localPosition[0], y+300, hsText.transform.localPosition[2]);
        centerText.transform.localPosition = new Vector3(centerText.transform.localPosition[0], y+300, centerText.transform.localPosition[2]);
        centerTextSelected.transform.localPosition = new Vector3(centerTextSelected.transform.localPosition[0], y+300, centerTextSelected.transform.localPosition[2]);
        /*if (numPlayers == 2)
        {
            if (playerNum == 1)
            {
                Debug.Log("0");
                posText.transform.localPosition = new Vector3(posText.transform.localPosition[0], y, posText.transform.localPosition[2]);
            } else if (playerNum == 2)
            { 
                Debug.Log("1");
            }
        }*/
    }
	public void setInfo(Vector3 info, int _numP)
    {
        if (numPlayers != _numP)
        {
            numPlayers = _numP;
            splitByNumPlayers();
        }
        else { 
            if (info[0] == -1 || info[1] == -1)
            {
                posText.text = "";

            }
            else
            {
                posText.text = info[0] + ", " + info[1];
                timeText.text = info[2].ToString();
            }
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
    public void splitTo()
    {
        
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
