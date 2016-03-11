using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkHandler : MonoBehaviour {
    private GameObject map;
    private chunk[,] chunks;
    public int maxX = -1;
    public int maxY = -1;
    public int renderDistance = 1;
    private HashSet<Transform>[] inside;
    private HashSet<Transform>[] outside;
    private bool flag;
    public ChunkHandler(GameObject _map)
    {
        map = _map;
        loadChunks();
        inside = new HashSet<Transform>[8];
        outside = new HashSet<Transform>[8];
        for (int i = 0; i < 8; i++)
        {
            inside[i] = new HashSet<Transform>();
            outside[i] = new HashSet<Transform>();
        }
        flag = true;
    }
    public int[] coordParse(string str)//turns string in format of "_0_2_" to a coordinate (0,2)
    {
        int[] ret;
        int firstInd = str.IndexOf("_", 0);
        int secondInd = str.IndexOf("_", firstInd+1);
        int thirdInd = str.IndexOf("_", secondInd+1);
        //System.Int32.Parse
        if (secondInd > 0 && thirdInd > 0)
        {
            string firstNum = (str.Substring(firstInd + 1, secondInd - firstInd - 1));
            string secondNum = (str.Substring(secondInd + 1, thirdInd - secondInd - 1));

            try
            {
                ret = new int[2] { System.Int32.Parse(firstNum), System.Int32.Parse(secondNum) };
            }
            catch (System.FormatException e)
            {
                Debug.Log("Your map was not named correctly for chunking");
                ret = new int[2] { 0, 0 };
            }
            //Debug.Log(firstNum + ", " + secondNum);
        }
        else
        {
            ret = new int[2] { -1, -1 };
        }
        return ret;
    }
    public void add(Transform t,int pNum)
    {
        int count = inside[pNum].Count;
        inside[pNum].Add(t);
        outside[pNum].Remove(t);
        flag = count == inside[pNum].Count && flag;
        act();
    }
    public void remove(Transform t,int pNum)
    {
        int count = outside[pNum].Count;
        outside[pNum].Add(t);
        inside[pNum].Remove(t);
        flag = count == outside[pNum].Count && flag;
        act();
    }
    public void act()
    {
        if (!flag)
        {
            flag = true;
            HashSet<Transform> toAdd = new HashSet<Transform>();
            HashSet<Transform> toSubtract = new HashSet<Transform>();
            foreach (HashSet<Transform> iins in inside)
            {
                toAdd.UnionWith(iins);
            }
            foreach (HashSet<Transform> iins in outside)
            {
                toSubtract.UnionWith(iins);
            }
            //outside.ExceptWith(toAdd);
            toSubtract.ExceptWith(toAdd);
            foreach (Transform t in toSubtract)
            {
                int[] vec = coordParse(t.gameObject.name);
                unload(vec);
            }
            foreach (Transform t in toAdd)
            {
                int[] vec = coordParse(t.gameObject.name);
                load(vec);
            }
        }
    }
    void loadChunks()
    {
        List<GameObject> children = new List<GameObject>();
        int numChildren = map.transform.childCount;
        for(int i = 0; i < numChildren; i++)
        {
            GameObject toAdd = map.transform.GetChild(i).gameObject;
            if (toAdd.name.Contains("_"))
            { 
                int[] vec = coordParse(toAdd.name);
                maxX = Mathf.Max(vec[0], maxX);
                maxY = Mathf.Max(vec[1], maxY);
                if (vec[0] != -1 && vec[1] != -1)
                {
                    children.Add(toAdd);
                }
            }
        }
        chunks = new chunk[maxX+1,maxY+1];
        for (int i = 0; i <= maxX; i++)
        {
            for (int j = 0; j <= maxY; j++)
            {
                chunks[i, j] = new chunk(this,new int[2] { i, j });
            }
        }
        foreach(GameObject g in children)
        {
            int[] vec =  coordParse(g.name);
            if (vec[0] != -1 && vec[1] != -1)
            {
                chunks[vec[0], vec[1]].add(g.transform);
            }
        }
    }
    public void load(int[] co)
    {
        if (co[0] != -1 && co[1] != -1)
        {
            foreach (int[] c in chunks[co[0], co[1]].neighbors)
            {
                chunks[c[0], c[1]].enable();
            }
        }
    }
    public void unloadAll()
    {
        foreach(chunk c in chunks)
        {
            c.disable();
        }
    }
    public void unload(int[] co)
    {
            chunks[co[0], co[1]].disable();
        
    }
    /* void handleStep(int[] prevChunk, int[] nextChunk)
    {
        //Debug.Log("handleStep called");
        HashSet<int[]> fromPrev = chunks[prevChunk[0], prevChunk[1]].neighbors;
        HashSet<int[]> fromNext = chunks[nextChunk[0], nextChunk[1]].neighbors;
        HashSet<int[]> toOff = fromPrev;
        toOff.ExceptWith(fromNext);
        HashSet<int[]> toOn = fromNext;
        toOn.ExceptWith(fromPrev);
        foreach(int[] c in toOff)
        {
            chunks[c[0], c[1]].disable();
        }
        foreach(int[] c in toOn)
        {
            chunks[c[0], c[1]].enable();
        }
    }*/
}
class chunk
{
    HashSet<Transform> ts;
    ChunkHandler owner;
    public HashSet<int[]> neighbors;
    int[] coord;
    public chunk(ChunkHandler _owner,int[] _coord)
    {
        coord = _coord;
        owner = _owner;
        ts = new HashSet<Transform>();
        loadNeighbors();
    }
    public void add(Transform t)
    {
        ts.Add(t);
    }
    public void enable()
    {
        foreach(Transform t in ts)
        {
            t.gameObject.SetActive(true);
        }
    }
    public void disable()
    {
        //Debug.Log("disabling " + coord[0] + ", " + coord[1]);
        foreach (Transform t in ts)
        {
            t.gameObject.SetActive(false);
        }
    }
    void loadNeighbors()
    {
        neighbors = new HashSet<int[]>();
        int xLowBound = Mathf.Max(0, coord[0] - owner.renderDistance);
        int xHighBound = Mathf.Min(coord[0] + owner.renderDistance + 1, owner.maxX+1);
        int yLowBound = Mathf.Max(0, coord[1] - owner.renderDistance);
        int yHighBound = Mathf.Min(coord[1] + owner.renderDistance + 1, owner.maxY+1);
        for(int i = xLowBound; i < xHighBound; i++)
        {
            for(int j = yLowBound; j < yHighBound; j++)
            {
                neighbors.Add(new int[2] { i, j });
            }
        }
    }
}
