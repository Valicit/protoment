using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    //This contains all the info for one wave of a dungeon.

    //These are our default values.
    public int level = -1;
    public int rank = -1;
    public int equipLevel = -1;

    //These are all our units, written this way to make it easier in the editor, even though code wise it's silly.
    public WaveUnit frontLine1 = new WaveUnit();
    public WaveUnit frontLine2 = new WaveUnit();
    public WaveUnit frontLine3 = new WaveUnit();
    public WaveUnit midLine1 = new WaveUnit();
    public WaveUnit midLine2 = new WaveUnit();
    public WaveUnit midLine3 = new WaveUnit();
    public WaveUnit backLine1 = new WaveUnit();
    public WaveUnit backLine2 = new WaveUnit();
    public WaveUnit backLine3 = new WaveUnit();

    //Construct my unit.
    public Unit GetUnit(WaveUnit wu, Dungeon d)
    {
        //Create our unit to hold the result.
        Unit r = null;
        if (wu.data.unit != null) r = Unit.NewUnit(wu.data.unit);
        else r = Unit.NewUnit(d.randomUnits[Random.Range(0, d.randomUnits.Count)]);

        //Set equips.
        EquipUnit(ref r, wu, d);

        //Set rank and level.
        int ulevel = 1;
        if (wu.data.level > 0) ulevel = wu.data.level - 1;
        else if (level > 0) ulevel = level - 1;
        else ulevel = d.unitLevel - 1;

        if (wu.data.rank > 0) r.rank = wu.data.rank;
        else if (rank > 0) r.rank = rank;
        else if (d.unitRank > 0) r.rank = d.unitRank;
        else r.rank = MathP.GetRank(ulevel);
        r.LevelUp(ulevel);
        

        //Heal the unit.
        r.cHP = r.GetmHP();

        //Return the result.
        return r;
    }

    //Equip the unit we're creating.
    private void EquipUnit(ref Unit r, WaveUnit wu, Dungeon d)
    {
        //Set our equipment.
        if (d.randEquip != null)
        {
            if (d.weaponEquipped)
            {
                r.weapon = Equipment.GetItem(d.randEquip, EquipType.Weapon);
                r.weapon.SetLevel(d.unitEquipLevel);
            }
            if (d.armorEquipped)
            {
                r.armor = Equipment.GetItem(d.randEquip, EquipType.Armor);
                r.armor.SetLevel(d.unitEquipLevel);
            }
            if (d.accessoryEquipped)
            {
                r.accessory = Equipment.GetItem(d.randEquip, EquipType.Accessory);
                r.accessory.SetLevel(d.unitEquipLevel);
            }
        }
        if (wu.data.weapon != null)
        {
            if (wu.data.weapon.WeaponMainStatsChoices.Find(n => n.stat == wu.data.weaponMainStat) != null) r.weapon = GetEquipment(wu.data.weapon, EquipType.Weapon, wu.data.weaponMainStat);
            else Debug.Log("Something went wrong getting unit equips in this dungeon. Guess: You selected the wrong main stat, and no item with a main stat like that can exist.");
            if (wu.data.equipLevel > 0) r.weapon.SetLevel(wu.data.equipLevel);
            else r.weapon.SetLevel(equipLevel);
        }
        if (wu.data.armor != null)
        {
            if (wu.data.armor.ArmorMainStatsChoices.Find(n => n.stat == wu.data.armorMainStat) != null) r.armor = GetEquipment(wu.data.armor, EquipType.Armor, wu.data.armorMainStat);
            else Debug.Log("Something went wrong getting unit equips in this dungeon. Guess: You selected the wrong main stat, and no item with a main stat like that can exist.");
            if (wu.data.equipLevel > 0) r.armor.SetLevel(wu.data.equipLevel);
            else r.armor.SetLevel(equipLevel);
        }
        if (wu.data.accessory != null)
        {
            if (wu.data.accessory.AccessoryMainStatsChoices.Find(n => n.stat == wu.data.accessoryMainStat) != null) r.accessory = GetEquipment(wu.data.accessory, EquipType.Accessory, wu.data.accessoryMainStat);
            else Debug.Log("Something went wrong getting unit equips in this dungeon. Guess: You selected the wrong main stat, and no item with a main stat like that can exist.");
            if (wu.data.equipLevel > 0) r.accessory.SetLevel(wu.data.equipLevel);
            else r.accessory.SetLevel(equipLevel);
        }
    }

    //Access by number.
    public Unit GetUnit(int i, Dungeon d)
    {
        switch (i)
        {
            case 0:
                return GetUnit(frontLine1, d);
            case 1:
                return GetUnit(frontLine2, d);
            case 2:
                return GetUnit(frontLine3, d);

            case 3:
                return GetUnit(midLine1, d);
            case 4:
                return GetUnit(midLine2, d);
            case 5:
                return GetUnit(midLine3, d);

            case 6:
                return GetUnit(backLine1, d);
            case 7:
                return GetUnit(backLine2, d);
            case 8:
                return GetUnit(backLine3, d);
        }
        Debug.Log("Something has gone terribly wrong in Wave.GetUnit(int)!");
        return null;
    }

    //Construct the equipment.
    public Equipment GetEquipment(Equipment e, EquipType et, EquipStats es)
    {
        //Roll until we get one.
        while (true)
        {
            Equipment r = Equipment.GetItem(e, et);
            if (r.MainStat.stat == es) return r;
        }
    }
}

//This is going to represent one unit in a wave when we're done.
[System.Serializable]
public class WaveUnit
{
    //This is the unit data for the unit.
    public bool filled;
    public UnitOverrideData data = new UnitOverrideData();  
}

[System.Serializable]
public class UnitOverrideData
{
    //These are variables for it.
    public UnitData unit;
    public int level = -1;
    public int rank = -1;
    public int equipLevel = -1;
    public Equipment weapon;
    public EquipStats weaponMainStat;
    public Equipment armor;
    public EquipStats armorMainStat;
    public Equipment accessory;
    public EquipStats accessoryMainStat;
}