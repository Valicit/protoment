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

    //Get the exp this party gives.
    public long GetEXP()
    {
        long r = 0;
        foreach (Unit u in GetAllUnits())
        {
            r += (long)Mathf.Round(u.GetENext(u.level) * 0.1f) + 6;
        }
        Debug.Log("Party EXP: " + r);
        return r;
    }

    #region Unit Finding Stuff.
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
    #endregion


    #region Skill targeting stuff.
    //Get a random single target.
    public List<Unit> ValidateSingleRandom()
    {
        if (GetAllLiving().Count > 0) return GetAllLiving();
        return new List<Unit>();
    }
    public Unit GetSingleRandom()
    {
        if (ValidateSingleRandom().Count > 0) return ValidateSingleRandom()[Random.Range(0, ValidateSingleRandom().Count)];
        else return null;
    }

    //Get a random target closest to the front line.
    public List<Unit> ValidateSingleFrontLine()
    {
        List<Unit> r = new List<Unit>();
        //If there is a unit on the very front line, add all living units on the front line. Then the next, then the next, till we find at least one.
        if (GetAllLiving().Contains(myUnits[0, 0]) || GetAllLiving().Contains(myUnits[0, 1]) || GetAllLiving().Contains(myUnits[0, 2]))
        {
            if (GetAllLiving().Contains(myUnits[0, 0])) r.Add(myUnits[0, 0]);
            if (GetAllLiving().Contains(myUnits[0, 1])) r.Add(myUnits[0, 1]);
            if (GetAllLiving().Contains(myUnits[0, 2])) r.Add(myUnits[0, 2]);
        }
        else if (GetAllLiving().Contains(myUnits[1, 0]) || GetAllLiving().Contains(myUnits[1, 1]) || GetAllLiving().Contains(myUnits[1, 2]))
        {
            if (GetAllLiving().Contains(myUnits[1, 0])) r.Add(myUnits[1, 0]);
            if (GetAllLiving().Contains(myUnits[1, 1])) r.Add(myUnits[1, 1]);
            if (GetAllLiving().Contains(myUnits[1, 2])) r.Add(myUnits[1, 2]);
        }
        else if (GetAllLiving().Contains(myUnits[2, 0]) || GetAllLiving().Contains(myUnits[2, 1]) || GetAllLiving().Contains(myUnits[2, 2]))
        {
            if (GetAllLiving().Contains(myUnits[2, 0])) r.Add(myUnits[2, 0]);
            if (GetAllLiving().Contains(myUnits[2, 1])) r.Add(myUnits[2, 1]);
            if (GetAllLiving().Contains(myUnits[2, 2])) r.Add(myUnits[2, 2]);
        }

        return r;
    }
    public Unit GetSingleFrontLine()
    {
        return ValidateSingleFrontLine()[Random.Range(0, ValidateSingleFrontLine().Count)];
    }

    //Get any units that share a row with the given unit.
    public List<Unit> GetUnitRow(Unit u)
    {
        //Get a list to hold our result.
        List<Unit> r = new List<Unit>();

        //Get the y of our unit.
        int y = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int z = 0; z < 3; z++)
            {
                if (myUnits[i, z] == u)
                {
                    y = z;
                }
            }
        }

        //Add those units to the list.
        if (GetAllLiving().Contains(myUnits[0, y])) r.Add(myUnits[0, y]);
        if (GetAllLiving().Contains(myUnits[1, y])) r.Add(myUnits[1, y]);
        if (GetAllLiving().Contains(myUnits[2, y])) r.Add(myUnits[2, y]);

        //Return the result.
        return r;
    }
    #endregion
}
