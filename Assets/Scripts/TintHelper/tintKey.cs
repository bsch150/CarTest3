using UnityEngine;
using System.Collections;

public class tintKey : MonoBehaviour {
    private Color[,] colors = new Color[,] { {Color.yellow, Color.green,Color.green,Color.blue},
                                              { Color.yellow,Color.blue,Color.blue,Color.blue },
                                              { Color.yellow,Color.blue,Color.blue,Color.blue },
                                              { Color.yellow,Color.blue,Color.blue,Color.blue }
    };
    public Color getColor(int x, int y)
    {
        return colors[x, y];
    }
    public Color getColor(int[] c)
    {
        Debug.Log("c = " + c);
        if (c[0] >= 0 && c[1] >= 0) { return colors[c[0], c[1]]; }
        else
        {
            return Color.white;
        }
    }

}
