using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class Dungeon : ScriptableObject
{
    //This controls one dungeon, and has all the variables needed to make it interesting.

    //This is an array of unit arrays that can be loaded into a party.
    public int currentWave = 0;
    public Wave[] waves;

    //These are only used for random dungeon creation.
    public bool isRandom;
    public List<UnitData> randomUnits;
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
        if (waves[wave].unit00 != null) p.myUnits[0, 0] = Unit.NewUnit(waves[wave].unit00, waves[wave].Level[0]);
        if (waves[wave].unit01 != null) p.myUnits[0, 1] = Unit.NewUnit(waves[wave].unit01, waves[wave].Level[1]);
        if (waves[wave].unit02 != null) p.myUnits[0, 2] = Unit.NewUnit(waves[wave].unit02, waves[wave].Level[2]);
        if (waves[wave].unit10 != null) p.myUnits[1, 0] = Unit.NewUnit(waves[wave].unit10, waves[wave].Level[3]);
        if (waves[wave].unit11 != null) p.myUnits[1, 1] = Unit.NewUnit(waves[wave].unit11, waves[wave].Level[4]);
        if (waves[wave].unit12 != null) p.myUnits[1, 2] = Unit.NewUnit(waves[wave].unit12, waves[wave].Level[5]);
        if (waves[wave].unit20 != null) p.myUnits[2, 0] = Unit.NewUnit(waves[wave].unit20, waves[wave].Level[6]);
        if (waves[wave].unit21 != null) p.myUnits[2, 1] = Unit.NewUnit(waves[wave].unit21, waves[wave].Level[7]);
        if (waves[wave].unit22 != null) p.myUnits[2, 2] = Unit.NewUnit(waves[wave].unit22, waves[wave].Level[8]);

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
            if (Random.Range(0, 100) < d.unitChance || key == 1) d.waves[i].unit00 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 2) d.waves[i].unit01 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 3) d.waves[i].unit02 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 4) d.waves[i].unit10 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 5) d.waves[i].unit11 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 6) d.waves[i].unit12 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 7) d.waves[i].unit20 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 8) d.waves[i].unit21 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            if (Random.Range(0, 100) < d.unitChance || key == 9) d.waves[i].unit22 = d.randomUnits[Random.Range(0, d.randomUnits.Count)];
            for (int z = 0; z < d.waves[i].Level.Length; z++)
            {
                d.waves[i].Level[z] = Dungeon.GetRandomEnemyLevel(i, d.baseLevel);
            }
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
