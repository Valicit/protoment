using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartyUnitSelect : MonoBehaviour
{
    //This is our button object.
    public GameObject buttonFab;

    //This is a reference to the frame where we add buttons as children.
    public Transform frame;

    //This is a list of the buttons we've already created.
    public List<UnitSelectButton> buttonList;

    //This is whether the list should show units in a party or not.
    public bool showParty;

    //This is what happens when we load the window.
    public void OnLoad()
    {
        //Sort the player party.
        Player.SortParty();
        //Destroy the old buttons and make new ones.
        DestroyButtons();
        CreateButtons();
    }

    //Destroy old buttons.
    public void DestroyButtons()
    {
        while (buttonList.Count > 0)
        {
            //Get our object.
            GameObject b = buttonList[0].gameObject;
            buttonList.RemoveAt(0);
            Destroy(b);
        }
    }

    //Create new buttons.
    public void CreateButtons()
    {
        //For each unit the player has.
        foreach (Unit u in Player.playerUnits)
        {
            //If the unit is not presently in a party.
            if (!Player.playerParty.GetAllUnits().Contains(u) || showParty)
            {
                AddButton(u);
            }
        }
    }

    //Add a button for an individual unit.
    public void AddButton(Unit u)
    {
        //make a new button.
        GameObject b = GameObject.Instantiate(buttonFab, frame) as GameObject;

        //Set the unit.
        b.GetComponent<UnitSelectButton>().myUnit = u;
        b.GetComponent<UnitSelectButton>().SetImage(u.uSprite);
        b.GetComponent<UnitSelectButton>().rankFrame.UpdateFrame(u.rank);
        b.GetComponent<UnitSelectButton>().txt_level.text = u.level.ToString();

        //Add it to the button list.
        buttonList.Add(b.GetComponent<UnitSelectButton>());
    }
}
