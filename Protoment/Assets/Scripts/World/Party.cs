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
            float levelMod = 1f / Mathf.Max(1, Mathf.Log(u.level));
            r += (long)Mathf.Round(u.GetENext(u.level) * Mathf.Max(levelMod, 0.1f) + 6);
        }
        return r;
    }

    //Forcibly delete a unit reference from the party.
    public void Delete(Unit u)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (myUnits[x, y] == u) myUnits[x, y] = null;
            }
        }
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

    //Get a random single target.
    public Unit GetSingleRandom()
    {
        if (ValidateSingleRandom().Count > 0) return ValidateSingleRandom()[Random.Range(0, ValidateSingleRandom().Count)];
        else return null;
    }

    //Get a single unit on the front lines.
    public Unit GetSingleFrontLine()
    {
        if (GetFrontLine().Count > 0) return GetFrontLine()[Random.Range(0, GetFrontLine().Count)];
        else return null;
    }

    //Get a single unit on the back lines.
    public Unit GetSingleBackLine()
    {
        if (GetBackLine().Count > 0) return GetBackLine()[Random.Range(0, GetBackLine().Count)];
        else return null;
    }

    //Get a line of units.
    public List<Unit> GetLine(int x)
    {
        List<Unit> r = new List<Unit>();
        if (GetAllLiving().Contains(myUnits[x, 0])) r.Add(myUnits[x, 0]);
        if (GetAllLiving().Contains(myUnits[x, 1])) r.Add(myUnits[x, 1]);
        if (GetAllLiving().Contains(myUnits[x, 2])) r.Add(myUnits[x, 2]);
        return r;
    }

    //Get the front line of units.
    public List<Unit> GetFrontLine()
    {
        List<Unit> r = new List<Unit>();
        //If there is a unit on the very front line, add all living units on the front line. Then the next, then the next, till we find at least one.
        if (GetAllLiving().Contains(myUnits[0, 0]) || GetAllLiving().Contains(myUnits[0, 1]) || GetAllLiving().Contains(myUnits[0, 2]))
        {
            r.AddRange(GetLine(0));
        }
        else if (GetAllLiving().Contains(myUnits[1, 0]) || GetAllLiving().Contains(myUnits[1, 1]) || GetAllLiving().Contains(myUnits[1, 2]))
        {
            r.AddRange(GetLine(1));
        }
        else if (GetAllLiving().Contains(myUnits[2, 0]) || GetAllLiving().Contains(myUnits[2, 1]) || GetAllLiving().Contains(myUnits[2, 2]))
        {
            r.AddRange(GetLine(2));
        }

        return r;
    }

    //Get the front line of units.
    public List<Unit> GetBackLine()
    {
        List<Unit> r = new List<Unit>();
        //If there is a unit on the very back line, add all living units on the back line. Then the next, then the next, till we find at least one.
        if (GetAllLiving().Contains(myUnits[2, 0]) || GetAllLiving().Contains(myUnits[2, 1]) || GetAllLiving().Contains(myUnits[2, 2]))
        {
            r.AddRange(GetLine(2));
        }
        else if (GetAllLiving().Contains(myUnits[1, 0]) || GetAllLiving().Contains(myUnits[1, 1]) || GetAllLiving().Contains(myUnits[1, 2]))
        {
            r.AddRange(GetLine(1));
        }
        else if (GetAllLiving().Contains(myUnits[0, 0]) || GetAllLiving().Contains(myUnits[0, 1]) || GetAllLiving().Contains(myUnits[0, 2]))
        {
            r.AddRange(GetLine(0));
        }

        return r;
    }

    //Get a row of units.
    public List<Unit> GetRow(int y)
    {
        List<Unit> r = new List<Unit>();
        if (GetAllLiving().Contains(myUnits[0, y])) r.Add(myUnits[0, y]);
        if (GetAllLiving().Contains(myUnits[1, y])) r.Add(myUnits[1, y]);
        if (GetAllLiving().Contains(myUnits[2, y])) r.Add(myUnits[2, y]);
        return r;
    }


    #endregion


    #region Skill targeting stuff.
    //Make sure a single target is valid.
    public List<Unit> ValidateSingleRandom()
    {
        if (GetAllLiving().Count > 0) return GetAllLiving();
        return new List<Unit>();
    }

    //Get the line that contains X unit.
    public List<Unit> GetLineContaining(Unit u)
    {
        //If our unit is on the front line.
        if (GetLine(0).Contains(u)) return GetLine(0);
        if (GetLine(1).Contains(u)) return GetLine(1);
        if (GetLine(2).Contains(u)) return GetLine(2);
        return new List<Unit>();
    }

    //Get any units that share a row with the given unit.
    public List<Unit> GetRowContaining(Unit u)
    {
        //If our unit is on the front line.
        if (GetRow(0).Contains(u)) return GetRow(0);
        if (GetRow(1).Contains(u)) return GetRow(1);
        if (GetRow(2).Contains(u)) return GetRow(2);
        return new List<Unit>();
    }

    //Get the lowest HP membet.
    public Unit GetLowestHP()
    {
        //Hold the result.
        Unit r = null;

        //For each living unit.
        for (int i = 0; i < GetAllLiving().Count; i++)
        {
            //If we have no unit yet, grab the first. Otherwise compare HP. If one is lower, take that one.
            if (r == null) r = GetAllLiving()[i];
            else if (((float)r.cHP / (float)r.mHP) > ((float)GetAllLiving()[i].cHP / (float)GetAllLiving()[i].mHP)) r = GetAllLiving()[i];
        }

        //Return the result.
        return r;
    }
    #endregion
}