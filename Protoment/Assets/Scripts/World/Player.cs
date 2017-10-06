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
    public static long mana;

    //These are summoning materials.
    public static int basicMaterial = 100000;
    public static int advancedMaterial = 0;
    public static int rainbowMaterial = 0;
    public static int luminousMaterial = 0;
    public static int lightShadowMaterial = 0;

    //Other materials.
    public static int itemWorldKeys = 10;

    //This is a reference to the current dungeon.
    public static Dungeon currentDungeon;
}