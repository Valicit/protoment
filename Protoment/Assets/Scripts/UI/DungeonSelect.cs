using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DungeonSelect : MonoBehaviour
{
    //This menu loads a dungeon to be the current dungeon.

    //This is a reference to the party frame.
    public PartyUnitSelect partyFrame;

    //This is a reference to our unit select buttons, so they can show the right info.
    public UnitSelectButton[,] buttons = new UnitSelectButton[3, 3];
    public UnitSelectButton[] bs;

    //This is the selected dungeon.
    public Dungeon selectedDungeon;

    //This is the currently selected unit.
    public Unit selectedUnit;

    //On startup.
    public void Start()
    {
        buttons[0, 0] = bs[0];
        buttons[0, 1] = bs[1];
        buttons[0, 2] = bs[2];
        buttons[1, 0] = bs[3];
        buttons[1, 1] = bs[4];
        buttons[1, 2] = bs[5];
        buttons[2, 0] = bs[6];
        buttons[2, 1] = bs[7];
        buttons[2, 2] = bs[8];
        partyFrame.OnLoad();
    }

    //This selects the dungeon.
    public void SelectDungeon(Dungeon d)
    {
        selectedDungeon = d;
    }

    //This method actually loads the dungeon.
    public void EnterDungeon()
    {
        Player.currentDungeon = Dungeon.Instantiate(selectedDungeon);
        SceneManager.LoadScene("Battle");
    }

    //Go back to town.
    public void Back()
    {
        SceneManager.LoadScene("Town");
    }

    //This happens when one of the party select buttons are pushed.
    public void PartySelectButton(int x, int y)
    {
        //If there is already a unit here.
        if (Player.playerParty.myUnits[x, y] != null)
        {
            Player.playerParty.myUnits[x, y] = null;
            partyFrame.OnLoad();
            buttons[x, y].SetImage(null);
        }
        else if(selectedUnit != null) //If there was no unit, and we have a selected unit.
        {
            
            Player.playerParty.myUnits[x, y] = selectedUnit;
            partyFrame.OnLoad();
            buttons[x, y].SetImage(selectedUnit.uSprite);
            selectedUnit = null;
        }
    }

    //This happens when we select a unit.
    public void OnUnitSelectButtonPressed(Unit u)
    {
        selectedUnit = u;
    }
}