using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Town : MonoBehaviour
{
    //This controls things the player can do in town!

    //This is our test dungeon.
    public Dungeon testDungeon;


    //Startup stuff.
    public void Start()
    {
        if (Player.playerParty == null)
        {
            Player.playerParty = new Party();
            Player.playerParty.myUnits[0, 0] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Fire")));
            Player.playerParty.myUnits[0, 1] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Water")));
            Player.playerParty.myUnits[0, 2] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Electric")));
            Player.playerParty.myUnits[2, 0] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Wood")));
            Player.playerParty.myUnits[2, 1] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Light")));
            Player.playerParty.myUnits[2, 2] = Unit.NewUnit((UnitData)Resources.Load(string.Format("Units/{0}/{1}", "Novice", "Dark")));
        }
    }

    //This starts a battle.
    public void StartBattle()
    {
        Player.currentDungeon = Dungeon.Instantiate(testDungeon);
        SceneManager.LoadScene("Battle");
    }
}
