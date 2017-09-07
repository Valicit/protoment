using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arena : MonoBehaviour
{
    //This Arena object contains a reference to the panels inside it, as well as methods for accessing them in various patterns.

    //These are the panels.
    public Panel[,] panels = new Panel[3, 3];
    public GameObject[] panels1D;

    //This is the party of characters here.
    public Party myParty = new Party();

    //This keeps track of whether this is the player side or the enemy side.
    public bool isPlayer;

    //On start!
    public void Start()
    {
        //Populate the list of panels from panels1D.
        PopulatePanelArray();
    }

    //Populate the 2D array, because Unity is a butt and won't let you do it in the Editor. WHAT NOW UNITY?
    public void PopulatePanelArray()
    {
        int i = 0;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                panels[x, y] = panels1D[i].GetComponent<Panel>();
                i++;
            }
        }
    }

    //Return a list of non-null units in this Arena.
    public List<Unit> GetUnits()
    {
        //Create a list to store our results.
        List<Unit> r = new List<Unit>();

        //For each panel,
        foreach (Panel p in panels)
        {
            if (p.myUnit != null)
            {
                r.Add(p.myUnit);
            }
        }

        //Return our result.
        return new List<Unit>();
    }
}
