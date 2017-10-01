using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipSelectPane : MonoBehaviour
{
    //This is our button object.
    public GameObject buttonFab;

    //This is a reference to the frame where we add buttons as children.
    public Transform frame;

    //This is a list of the buttons we've already created.
    public List<EquipSelectButton> buttonList;

    //This is what type of equipment we are currently showing.
    public EquipType eType;

    //This is what happens when we load the window.
    public void OnLoad()
    {
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
        foreach (Equipment e in Player.playerEquips.FindAll(n => n.equipType == eType))
        {
            //Add a button for it.
            AddButton(e);
        }
    }

    //Add a button for an individual unit.
    public void AddButton(Equipment e)
    {
        //make a new button.
        GameObject b = GameObject.Instantiate(buttonFab, frame) as GameObject;

        //Set the unit.
        b.GetComponent<EquipSelectButton>().myEquip = e;
        b.GetComponent<EquipSelectButton>().SetText(e.MainStat.stat.ToString());

        //Add it to the button list.
        buttonList.Add(b.GetComponent<EquipSelectButton>());
    }
}
