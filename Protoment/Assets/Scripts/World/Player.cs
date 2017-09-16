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

    //This is a reference to the current dungeon.
    public static Dungeon currentDungeon;
}
