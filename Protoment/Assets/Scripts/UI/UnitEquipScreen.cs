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
    public Text txt_ItemWorldButton;
    public Text txt_SellItemButton;

    //These are references to other things in the scene.
    public UnitStatusScreen uStatus;
    public EquipSelectPane itemPane;

    //These are dungeon prefabs for all sorts of items.
    public Dungeon commonFab;
    public Dungeon uncommonFab;
    public Dungeon rareFab;
    public Dungeon epicFab;
    public Dungeon legendFab;

	// Use this for initialization
	void Awake () {
        OnEquippedButtonPress(0);
	}
	
	// Update is called once per frame
	void Update () {
        UpdateEquipmentText(uStatus.myUnit.GetEquipped(typeSelected), MainStatEquippedText, SubstatEquippedText);
        UpdateEquipmentText(selectedInventory, MainStatSelectedText, SubstatSelectedText);

        //Update other stuff.
        UpdateButtons();
	}

    //Update buttons and stuff.
    public void UpdateButtons()
    {
        if (selectedInventory != null)
        {
            txt_ItemWorldButton.text = string.Format("Item World: {0} / {1} Keys", Player.itemWorldKeys, GetItemWorldPrice(selectedInventory.rarity));
            txt_SellItemButton.text = string.Format("Sell Item: {0} Keys", GetItemWorldPrice(selectedInventory.rarity) / 10);
        }
        else
        {
            txt_ItemWorldButton.text = string.Format("Select an item.");
            txt_SellItemButton.text = string.Format("Select an item.");
        }
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
            if (new List<EquipStats> { EquipStats.AGI, EquipStats.Crit, EquipStats.CritDMG, EquipStats.DEF, EquipStats.DEX, EquipStats.EXP, EquipStats.Healing, EquipStats.HP, EquipStats.INT, EquipStats.Regen, EquipStats.SPR, EquipStats.STR, EquipStats.Thorns }.Contains(selected.MainStat.stat))
            {
                MainStat.text += "%";
            }
            MainStat.text = MainStat.text.Replace("Flat", "");
            MainStat.text += string.Format(" Lv.{0} ({1})", selected.level, selected.rarity);

            //For each substat slot.
            for (int i = 0; i < 8; i++)
            {
                //If there is a substat here.
                if (i < selected.SubStats.Count)
                {
                    //Set the text.
                    SubStats[i].gameObject.SetActive(true);
                    SubStats[i].text = selected.SubStats[i].stat + ": +" + selected.GetComponentValue(selected.SubStats[i]);
                    if (new List<EquipStats> { EquipStats.AGI, EquipStats.Crit, EquipStats.CritDMG, EquipStats.DEF, EquipStats.DEX, EquipStats.EXP, EquipStats.Healing, EquipStats.HP, EquipStats.INT, EquipStats.Regen, EquipStats.SPR, EquipStats.STR, EquipStats.Thorns }.Contains(selected.SubStats[i].stat))
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
        if(selectedInventory != null) if (selectedInventory.equipType != typeSelected) selectedInventory = null;
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
            Dungeon d = null;
            switch (selectedInventory.rarity)
            {
                case Rarity.Common:
                    d = Dungeon.CreateRandom(commonFab, selectedInventory);
                    break;
                case Rarity.Uncommon:
                    d = Dungeon.CreateRandom(uncommonFab, selectedInventory);
                    break;
                case Rarity.Rare:
                    d = Dungeon.CreateRandom(rareFab, selectedInventory);
                    break;
                case Rarity.Epic:
                    d = Dungeon.CreateRandom(epicFab, selectedInventory);
                    break;
                case Rarity.Legendary:
                    d = Dungeon.CreateRandom(legendFab, selectedInventory);
                    break;
            }
            if (Player.itemWorldKeys >= GetItemWorldPrice(selectedInventory.rarity))
            {
                Player.itemWorldKeys -= GetItemWorldPrice(selectedInventory.rarity);
                Player.currentDungeon = d;
                Player.currentDungeon.currentWave = Player.currentDungeon.itemWorldEquip.level - 1;
                SceneManager.LoadScene("Battle");
            }
        }
    }

    //Get the cost of item world.
    public int GetItemWorldPrice(Rarity r)
    {
        switch (r)
        {
            case Rarity.Common:
                return 10;
            case Rarity.Uncommon:
                return 40;
            case Rarity.Rare:
                return 90;
            case Rarity.Epic:
                return 160;
            case Rarity.Legendary:
                return 250;
        }
        return 10;
    }

    //Sell an item to get rid of it.
    public void SellItem()
    {
        //If there's an item selected.
        if (selectedInventory != null)
        {
            //Get rid of it.
            Player.playerEquips.Remove(selectedInventory);
            Player.itemWorldKeys += GetItemWorldPrice(selectedInventory.rarity) / 10;
            selectedInventory = null;
            itemPane.OnLoad();
        }
    }
}
