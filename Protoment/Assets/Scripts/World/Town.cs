using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Town : MonoBehaviour
{
    //This controls things the player can do in town!

    //This is our test dungeon.
    public Dungeon testDungeon;

    //This is a reference to the starter character.
    public List<UnitData> starters;
    public Equipment commonEquip;

    //Startup stuff.
    public void Start()
    {
        //If the players unit list is empty, spawn the starter character.
        if (Player.playerUnits.Count == 0)
        {
            foreach (UnitData ud in starters)
            {
                Player.playerUnits.Add(Unit.NewUnit(ud, 1));
            }
            Player.playerParty.myUnits[1, 1] = Player.playerUnits[0];

            Player.playerEquips.Add(Equipment.GetItem(commonEquip, EquipType.Weapon));
            //Player.playerEquips[0].SetLevel(100);
            //Player.playerEquips[0].SubStats.Add(new EquipComponent() { stat = EquipStats.Crit, value = 10000 });
            //Player.playerEquips[0].SubStats.Add(new EquipComponent() { stat = EquipStats.CritDMG, value = 10000 });
        }
    }

    //This starts a battle.
    public void StartBattle()
    {
        Player.currentDungeon = Dungeon.Instantiate(testDungeon);
        SceneManager.LoadScene("DungeonSelect");
    }
}
