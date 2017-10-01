using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This controls equipment type.
public enum EquipType
{
    Weapon,
    Armor,
    Accessory
}

//these are equipment stats.
public enum EquipStats
{
    HP,
    STR,
    DEF,
    INT,
    SPR,
    DEX,
    AGI,
    Crit,
    CritDMG,
    Speed,
    FlatHP,
    FlatSTR,
    FlatDEF,
    FlatINT,
    FlatSPR,
    FlatDEX,
    FlatAGI,
    Thorns,
    FlatThorns,
    Healing,
    Regen,
    FlatRegen,
    EXP,
    Mana
}

[CreateAssetMenu]
public class Equipment : ScriptableObject
{
    //This controls what kind of equipment we are.
    public EquipType equipType;

    //These are the stats on the equipment. 0 is the main stat, and the other 8 are substats.
    public EquipComponent MainStat;
    public List<EquipComponent> SubStats;
    public int substatCount = 2;
    public int level = 1;

    //This is the list of possible main stats on different kinds of equipments.
    public List<EquipComponent> WeaponMainStatsChoices;
    public List<EquipComponent> ArmorMainStatsChoices;
    public List<EquipComponent> AccessoryMainStatsChoices;
    public List<EquipComponent> SubStatsChoices;

    //Get a stat value.
    public float GetStatValue(EquipStats es)
    {
        //Is this stat our main stat?
        if (MainStat.stat == es) return GetComponentValue(MainStat);
        else
        {
            //Is this any of our substats?
            if (SubStats.Find(n => n.stat == es) != null) return GetComponentValue(SubStats.Find(n => n.stat == es));
        }
        //Otherwise, return 0.
        return 0;
    }

    //Get the actual stat value of a stat.
    public float GetComponentValue(EquipComponent c)
    {
        //Items start with 1% potential.
        float r = (c.value * 0.01f);// * (float)level;

        //The remaining 99% is as follows.
        float p = (c.value * 0.99f);
        r += p * ((level * level) * 0.0001f);

        r = Mathf.CeilToInt(r);
        return r;
    }

    //Get an item.
    public static Equipment GetItem(Equipment e, EquipType et)
    {
        //Hold our result.
        Equipment r = Equipment.Instantiate(e);

        //Generate stats.
        switch (et)
        {
            case EquipType.Weapon:
                r.MainStat = r.WeaponMainStatsChoices[Random.Range(0, r.WeaponMainStatsChoices.Count)];
                break;
            case EquipType.Armor:
                r.MainStat = r.ArmorMainStatsChoices[Random.Range(0, r.ArmorMainStatsChoices.Count)];
                break;
            case EquipType.Accessory:
                r.MainStat = r.AccessoryMainStatsChoices[Random.Range(0, r.AccessoryMainStatsChoices.Count)];
                break;
        }
        r.MainStat.value = Mathf.Round(Random.Range(r.MainStat.minValue, r.MainStat.maxValue));
        r = GenerateSubstats(r);
        r.equipType = et;

        return r;
    }
    public static Equipment GetItem(Equipment e)
    {
        return GetItem(e, (EquipType)Random.Range(0, 3));
    }

    //Generate substats for equipment.
    public static Equipment GenerateSubstats(Equipment r)
    {
        //Roll substats.
        for (int i = 0; i < r.substatCount;)
        {
            //Get a new substat.
            EquipComponent c = r.SubStatsChoices[Random.Range(0, r.SubStatsChoices.Count)];

            //If we don't already have a substat with this.
            if (r.SubStats.Find(n => n.stat == c.stat) == null)
            {
                c.value = Mathf.Round(Random.Range(c.minValue, c.maxValue));
                r.SubStats.Add(c);
                i++;
            }
        }

        return r;
    }
}

[System.Serializable]
public class EquipComponent
{
    //This is the stat and value pair.
    public EquipStats stat;
    public float value;
    public float minValue;
    public float maxValue;
}
