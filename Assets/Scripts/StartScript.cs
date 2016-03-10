using UnityEngine;
using System.Collections;
using InputPlusControl;
using System;

public class StartScript : MonoBehaviour {
    UIScript ui;
    string[] contents;
    Action[] actions;
    private int selected = 0;
    bool showing = false;
    PlayerController player;
	public StartScript(UIScript _ui, PlayerController p)
    {
        player = p;
        ui = _ui;
        contents = new string[3]
        {
            "Back to Game",
            "Leave Track",
            "Quit"
        };
        actions = new Action[3]{
            returnToGame,
            leaveTrack,
            quit
        };
    }
    void returnToGame()
    {
        hide();
    }
    void leaveTrack()
    {
        player.leaveTrack();
        returnToGame();
    }
    void quit()
    {

    }
    string getSelected()
    {
        string ret = "";
        for(int i = 0; i < contents.Length; i++)
        {
            if(i == selected)
            {
                ret += contents[i] + "\n";
            }
            else
            {
                ret += "\n";
            }
        }
        return ret;
    }
    public void show()
    {
        showing = true;
        string str = "";
        foreach(string s in contents)
        {
            str += s + "\n";
        }
        Debug.Log("show called");
        ui.centerText.text = str;
        ui.centerTextSelected.text = getSelected();
        ui.toggleTimer = Time.time;
    }
    void moveUp()
    {
        selected--;
        if(selected < 0)
        {
            selected = contents.Length - 1;
        }
        show();
    }
    void moveDown()
    {
        selected++;
        if(selected >= contents.Length)
        {
            selected = 0;
        }
        show();
    }
    public void update()
    {
        if (showing)
        {
            float inp = -InputPlus.GetData(ui.controllerNum + 1, ControllerVarEnum.dpad_down) + InputPlus.GetData(ui.controllerNum + 1, ControllerVarEnum.dpad_up);
            float rightInp = InputPlus.GetData(ui.controllerNum + 1, ControllerVarEnum.dpad_right);
            if (rightInp == 0)
            {
                Debug.Log("inp = " + inp);
                if (inp > 0)
                {
                    Debug.Log("inp = " + inp);
                    moveUp();
                }
                else if (inp < 0)
                {
                    Debug.Log("inp = " + inp);
                    moveDown();
                }
            }
            else
            {
                actions[selected]();
            }
        }
    }
    public void hide()
    {
        showing = false;
        ui.centerText.text = "";
        ui.centerTextSelected.text = "";
    }
}
