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
}

//These identify substat groups.
public enum SubstatValueGroup
{
    FlatHP,
    FlatStat,
    PercentageStat,
    Crit,
    CritDMG,
    Thorns,
    FlatThorns,
    Regen,
    FlatRegen,
    EXP,
    Healing,
    Speed
}

[CreateAssetMenu]
public class Equipment : ScriptableObject
{
    //This controls what kind of equipment we are.
    public EquipType equipType;
    public Rarity rarity;

    //These are the stats on the equipment. 0 is the main stat, and the other 8 are substats.
    public EquipComponent MainStat;
    public List<EquipComponent> SubStats;
    public int substatCount = 2;
    public int level = 1;

    //These are substat values.
    public Vector2 subFlatHP;
    public Vector2 subFlatStat;
    public Vector2 subPercentageStat;
    public Vector2 subCrit;
    public Vector2 subCritDMG;
    public Vector2 subThorns;
    public Vector2 subFlatThorns;
    public Vector2 subRegen;
    public Vector2 subFlatRegen;
    public Vector2 subEXP;
    public Vector2 subHealing;
    public Vector2 subSpeed;

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

    //Get a substat value group.
    public Vector2 GetSubstatVectorGroup(SubstatValueGroup g)
    {
        switch (g)
        {
            case SubstatValueGroup.FlatHP:
                return subFlatHP;
            case SubstatValueGroup.FlatStat:
                return subFlatStat;
            case SubstatValueGroup.PercentageStat:
                return subPercentageStat;
            case SubstatValueGroup.Crit:
                return subCrit;
            case SubstatValueGroup.CritDMG:
                return subCrit;
            case SubstatValueGroup.Thorns:
                return subThorns;
            case SubstatValueGroup.FlatThorns:
                return subFlatThorns;
            case SubstatValueGroup.Regen:
                return subRegen;
            case SubstatValueGroup.FlatRegen:
                return subFlatRegen;
            case SubstatValueGroup.EXP:
                return subEXP;
            case SubstatValueGroup.Healing:
                return subHealing;
            case SubstatValueGroup.Speed:
                return subSpeed;
        }
        return new Vector2();
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
        Vector2 v = r.GetSubstatVectorGroup(r.MainStat.statGroup);
        r.MainStat.value = Mathf.Round(Random.Range(v.x * 4, v.y * 4));
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
        while (r.SubStats.Count < Mathf.FloorToInt((float)r.level / 10f) && r.SubStats.Count < r.substatCount)
        {
            //Get a new substat.
            EquipComponent c = r.SubStatsChoices[Random.Range(0, r.SubStatsChoices.Count)];

            //If we don't already have a substat with this.
            if (r.SubStats.Find(n => n.stat == c.stat) == null || c.stat == EquipStats.EXP) // Known bug: Duplicate rolls of a stat like EXP always have the same value for some reason.
            {
                Vector2 v = r.GetSubstatVectorGroup(c.statGroup);
                c.value = Mathf.Round(Random.Range(v.x, v.y));
                r.SubStats.Add(c);
            }
        }

        return r;
    }

    //Level up the item.
    public void SetLevel(int nlevel)
    {
        level = nlevel;
        GenerateSubstats(this);
    }
}

[System.Serializable]
public class EquipComponent
{
    //This is the stat and value pair.
    public EquipStats stat;
    public SubstatValueGroup statGroup;
    public float value;
}
