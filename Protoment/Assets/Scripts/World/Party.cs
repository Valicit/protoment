using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Party
{
    //A party represents a group of characters, holds information on their placements and contains methods for accessing them in various patterns or through various conditions.

    //This is the main array of units this party represents.
    public Unit[,] myUnits = new Unit[3, 3];

    //Tick forward each of the units.
    public void Tick()
    {
        //Get a list of living party members.
        List<Unit> uList = GetAllLiving();

        //For each living party member.
        for (int i = 0; i < uList.Count; i++)
        {
            uList[i].Tick();
        }
    }

    //Get a list of all units.
    public List<Unit> GetAllUnits()
    {
        //Hold our result.
        List<Unit> r = new List<Unit>();

        //For each party slot, if there's a character, add it to the list.
        foreach (Unit u in myUnits)
        {
            if (u != null) r.Add(u);
        }

        //Return the result.
        return r;
    }

    //Get a list of all living units.
    public List<Unit> GetAllLiving()
    {
        return GetAllUnits().FindAll(n => n.IsAlive());
    }

    //Get a list of units that are ready to take a turn.
    public List<Unit> GetAllReady()
    {
        return GetAllLiving().FindAll(n => n.TurnReady());
    }
}
