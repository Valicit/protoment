using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Town : MonoBehaviour
{
    //This controls things the player can do in town!

    //This is our test dungeon.
    public Dungeon testDungeon;

    //This is a list of units we can potentially summon.
    public List<UnitData> summonList;

    //This is a reference to the starter character.
    public UnitData starter;

    //Startup stuff.
    public void Start()
    {
        //If the players unit list is empty, spawn the starter character.
        if (Player.playerUnits.Count == 0) Player.playerUnits.Add(Unit.NewUnit(starter, 9999));
    }

    //This starts a battle.
    public void StartBattle()
    {
        Player.currentDungeon = Dungeon.Instantiate(testDungeon);
        SceneManager.LoadScene("DungeonSelect");
    }

    //Summon a unit.
    public void SummonUnit()
    {
        Player.playerUnits.Add(Unit.NewUnit(summonList[Random.Range(0, summonList.Count)]));
    }
}
