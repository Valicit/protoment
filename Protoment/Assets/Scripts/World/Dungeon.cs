using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class Dungeon : ScriptableObject
{
    //This controls one dungeon, and has all the variables needed to make it interesting.

    //These are primary dungeon values.
    public int unitLevel = 1;
    public int unitRank = -1;
    public int unitEquipLevel = 1;
    public Equipment randEquip;
    public bool weaponEquipped;
    public bool armorEquipped;
    public bool accessoryEquipped;
    public bool isRandom;
    public List<UnitData> randomUnits;

    //This is an array of unit arrays that can be loaded into a party.
    public int currentWave = 0;
    public Wave[] waves;

    //These are only used for random dungeon creation.
    public int baseLevel = 1;
    public int floorCount = 100;
    public float unitChance = 100f;
    public Equipment itemWorldEquip;

    //These are other dungeon values.
    public Reward[] rewards;

    //Get the current wave.
    public Party GetParty(int wave)
    {
        //Make a new party.
        Party p = new Party();

        //Insert Units. There was no reason not to use a nested loop here. But I am tired and I realized half way through making it. Go me.
        if (waves[wave].frontLine1.filled) p.myUnits[0, 0] = waves[wave].GetUnit(0, this);
        if (waves[wave].frontLine2.filled) p.myUnits[0, 1] = waves[wave].GetUnit(1, this);
        if (waves[wave].frontLine3.filled) p.myUnits[0, 2] = waves[wave].GetUnit(2, this);
        if (waves[wave].midLine1.filled) p.myUnits[1, 0] = waves[wave].GetUnit(3, this);
        if (waves[wave].midLine2.filled) p.myUnits[1, 1] = waves[wave].GetUnit(4, this);
        if (waves[wave].midLine3.filled) p.myUnits[1, 2] = waves[wave].GetUnit(5, this);
        if (waves[wave].backLine1.filled) p.myUnits[2, 0] = waves[wave].GetUnit(6, this);
        if (waves[wave].backLine2.filled) p.myUnits[2, 1] = waves[wave].GetUnit(7, this);
        if (waves[wave].backLine3.filled) p.myUnits[2, 2] = waves[wave].GetUnit(8, this);

        //Return the party.
        return p;
    }

    //Grant a reward for completion.
    public string GrantReward()
    {
        //Roll our random value.
        float r = Random.Range(0, 100);
        //This is for a cumulative value until we get something.
        float v = 0;

        //Run a loop until we grant a reward.
        for (int i = 0; i < rewards.Length; i++)
        {
            v += rewards[i].chances;
            if (r < v)
            {
                return rewards[i].GrantReward();
            }
        }

        return "No rewards set up for this dungeon yet. Sorry.";
    }

    //Create a random dungeon.
    public static Dungeon CreateRandom(Dungeon fab, Equipment e)
    {
        //For each future wave.
        Dungeon d = Dungeon.Instantiate(fab);

        //Set values.
        d.waves = new Wave[d.floorCount];
        d.itemWorldEquip = e;

        //For each wave.
        for (int i = 0; i < d.waves.Length; i++)
        {
            d.waves[i] = new Wave();
            int key = Random.Range(1, 10);
            if (Random.Range(0, 100) < d.unitChance || key == 1) d.waves[i].frontLine1.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 2) d.waves[i].frontLine2.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 3) d.waves[i].frontLine3.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 4) d.waves[i].midLine1.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 5) d.waves[i].midLine2.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 6) d.waves[i].midLine3.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 7) d.waves[i].backLine1.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 8) d.waves[i].backLine2.filled = true;
            if (Random.Range(0, 100) < d.unitChance || key == 9) d.waves[i].backLine3.filled = true;
            d.waves[i].level = Dungeon.GetRandomEnemyLevel(i, d.baseLevel);
            d.waves[i].equipLevel = Mathf.CeilToInt(Mathf.Max(1, d.waves[i].level * 0.01f));
        }

        //Return the result.
        return d;
    }

    //Get the level of an enemy on a random floor.
    public static int GetRandomEnemyLevel(int wave, float rankMod)
    {
        //var Base_Level = Math.floor((Item_Rank + 5) * (Item_Rank + 6) * 0.05) + (Math.floor(Item_Rank / 39) * 51) + (Math.floor(Item_Rank / 40) * 146);
        //var Average_Level = Base_Level * (Item_Floor + 9 + 5 * Math.floor((Item_Floor - 1) * 0.1)) * 0.10;
        int baseLevel = Mathf.FloorToInt(Mathf.Floor((rankMod + 5) * (rankMod + 6) * 0.05f) + (Mathf.Floor(rankMod / 39) * 51) + (Mathf.Floor(rankMod / 40) * 146));
        int r = Mathf.FloorToInt(baseLevel * ((float)wave + 9 + 5 * Mathf.Floor(((float)wave - 1) * 0.1f)) * 0.1f);
        //Calculate the average level for the floor/

        //return the level, randomized slightly.
        return Mathf.RoundToInt((float)r * Random.Range(0.85f, 1.15f));
    }
}
