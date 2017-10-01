using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UnitEquipScreen : MonoBehaviour
{
    //These are the selected equipments.
    public Equipment selectedInventory;
    public EquipType typeSelected;

    //These are references to the UI elements on this screen.
    public Text MainStatEquippedText;
    public Text MainStatSelectedText;
    public Text[] SubstatEquippedText;
    public Text[] SubstatSelectedText;

    //These are references to other things in the scene.
    public UnitStatusScreen uStatus;
    public EquipSelectPane itemPane;

    //These are dungeon prefabs for all sorts of items.
    public Dungeon commonFab;

	// Use this for initialization
	void Awake () {
        OnEquippedButtonPress(0);
	}
	
	// Update is called once per frame
	void Update () {
        UpdateEquipmentText(uStatus.myUnit.GetEquipped(typeSelected), MainStatEquippedText, SubstatEquippedText);
        UpdateEquipmentText(selectedInventory, MainStatSelectedText, SubstatSelectedText);
	}

    //Update the UI elements.
    public void UpdateEquipmentText(Equipment selected, Text MainStat, Text[] SubStats)
    {
        //If I've got a selected equipment.
        if (selected != null)
        {
            //Set main stat text.
            MainStat.gameObject.SetActive(true);
            MainStat.text = selected.MainStat.stat + ": +" + selected.GetComponentValue(selected.MainStat);
            if (new List<EquipStats> { EquipStats.AGI, EquipStats.Crit, EquipStats.CritDMG, EquipStats.DEF, EquipStats.DEX, EquipStats.EXP, EquipStats.Healing, EquipStats.HP, EquipStats.INT, EquipStats.Mana, EquipStats.Regen, EquipStats.SPR, EquipStats.STR, EquipStats.Thorns }.Contains(selected.MainStat.stat))
            {
                MainStat.text += "%";
            }
            MainStat.text = MainStat.text.Replace("Flat", "");
            MainStat.text += " Lv." + selected.level;

            //For each substat slot.
            for (int i = 0; i < 8; i++)
            {
                //If there is a substat here.
                if (i < selected.SubStats.Count)
                {
                    //Set the text.
                    SubStats[i].gameObject.SetActive(true);
                    SubStats[i].text = selected.SubStats[i].stat + ": +" + selected.GetComponentValue(selected.SubStats[i]);
                    if (new List<EquipStats> { EquipStats.AGI, EquipStats.Crit, EquipStats.CritDMG, EquipStats.DEF, EquipStats.DEX, EquipStats.EXP, EquipStats.Healing, EquipStats.HP, EquipStats.INT, EquipStats.Mana, EquipStats.Regen, EquipStats.SPR, EquipStats.STR, EquipStats.Thorns }.Contains(selected.SubStats[i].stat))
                    {
                        SubStats[i].text += "%";
                    }
                    SubStats[i].text = SubStats[i].text.Replace("Flat", "");
                }
                else
                {
                    //Otherwise, hide it.
                    SubStats[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            //Set all of them inactive.
            MainStat.gameObject.SetActive(false);
            for (int i = 0; i < SubstatEquippedText.Length; i++)
            {
                SubStats[i].gameObject.SetActive(false);
            }
        }
    }

    //This is what happens when an inventory item is selected.
    public void OnInventorySelect(Equipment e)
    {
        selectedInventory = e;
    }

    //This is what happens when the equipped item buttons are pressed.
    public void OnEquippedButtonPress(int et)
    {
        typeSelected = (EquipType)et;
        itemPane.eType = typeSelected;
        itemPane.OnLoad();
    }

    //This actually equips the item.
    public void Equip()
    {
        //Make sure we're working with a unit that exists.
        if (uStatus.myUnit != null)
        {
            //Based on what type of equipment it is.
            switch (typeSelected)
            {
                case EquipType.Weapon:
                    if(uStatus.myUnit.weapon != null) Player.playerEquips.Add(uStatus.myUnit.weapon);
                    uStatus.myUnit.weapon = selectedInventory;
                    Player.playerEquips.Remove(selectedInventory);
                    break;

                case EquipType.Armor:
                    if (uStatus.myUnit.armor != null) Player.playerEquips.Add(uStatus.myUnit.armor);
                    uStatus.myUnit.armor = selectedInventory;
                    Player.playerEquips.Remove(selectedInventory);
                    break;

                case EquipType.Accessory:
                    if (uStatus.myUnit.accessory != null) Player.playerEquips.Add(uStatus.myUnit.accessory);
                    uStatus.myUnit.accessory = selectedInventory;
                    Player.playerEquips.Remove(selectedInventory);
                    break;
            }

            //Unselect everything.
            selectedInventory = null;
            itemPane.OnLoad();
        }
    }

    //Unequip the current item without putting something new there.
    public void Unequip()
    {
        if (uStatus.myUnit.GetEquipped(typeSelected) != null)
        {
            Player.playerEquips.Add(uStatus.myUnit.GetEquipped(typeSelected));
            switch (typeSelected)
            {
                case EquipType.Weapon:
                    uStatus.myUnit.weapon = null;
                    break;
                case EquipType.Armor:
                    uStatus.myUnit.armor = null;
                    break;
                case EquipType.Accessory:
                    uStatus.myUnit.accessory = null;
                    break;
            }
        }
    }

    //Go to item world on the selected equipment.
    public void ItemWorld()
    {
        //If we have a selected equipment.
        if (selectedInventory != null)
        {
            Player.currentDungeon = Dungeon.CreateRandom(commonFab, selectedInventory);
            SceneManager.LoadScene("Battle");
        }
    }

    //Sell an item to get rid of it.
    public void SellItem()
    {
        //If there's an item selected.
        if (selectedInventory != null)
        {
            //Get rid of it.
            Player.playerEquips.Remove(selectedInventory);
            selectedInventory = null;
            itemPane.OnLoad();
        }
    }
}
