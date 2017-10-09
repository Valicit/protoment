using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
    //This class serves to have a handy reference to all player data.

    //This is the players currently selected party.
    public static Party playerParty = new Party();

    //This is a list of every unit the player owns.
    public static List<Unit> playerUnits = new List<Unit>();

    //This is a list of equipment the player has.
    public static List<Equipment> playerEquips = new List<Equipment>();

    //This is the player's mana. Used for upgrading units.
    public static long[] imagination = new long[10];

    //These are summoning materials.
    public static int basicMaterial = 0;
    public static int advancedMaterial = 0;
    public static int rainbowMaterial = 0;
    public static int luminousMaterial = 0;
    public static int lightShadowMaterial = 0;

    //Other materials.
    public static int itemWorldKeys = 0;

    //This is a reference to the current dungeon.
    public static Dungeon currentDungeon;

    //Sort the player party.
    public static void SortParty()
    {
        playerUnits.Sort(new PartyComparer());
    }
}

//This is a sorter that sorts the party.
public class PartyComparer : IComparer<Unit>
{
    //Compare two units.
    public int Compare(Unit x, Unit y)
    {
        //First sort by rank.
        if (x.rank > y.rank)
        {
            return -1;
        }
        else if (y.rank > x.rank)
        {
            return 1;
        }
        //Next, sort by Rarity.
        else if ((int)x.uRarity > (int)y.uRarity)
        {
            return -1;
        }
        else if ((int)y.uRarity > (int)x.uRarity)
        {
            return 1;
        }
        //Next sort by level.
        else if (x.level > y.level)
        {
            return -1;
        }
        else if (y.level > x.level)
        {
            return 1;
        }
        //Next sort by job.
        else if (x.job != y.job)
        {
            return x.job.CompareTo(y.job);
        }
        //Next sort by element.
        else if (x.uElement != y.uElement)
        {
            if ((int)x.uElement > (int)y.uElement) return 1;
            else return -1;
        }
        
        //If we've reached this point, there is nothing left to compare. They're the same in every way.
        return 0;
    }
}