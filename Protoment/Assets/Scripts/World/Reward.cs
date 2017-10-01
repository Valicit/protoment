using UnityEngine;
using System.Collections;

public enum RewardType
{
    Nothing,
    Item,
    BasicMaterial,
    AdvancedMaterial,
    RainbowMaterial,
    LuminousMaterial,
    LightShadowMaterial,
    Weapon,
    Armor,
    Accessory
}

[System.Serializable]
public class Reward
{
    //This is our reward type.
    public RewardType rewardType;

    //These are the chances of getting this reward.
    public float chances = 0;

    //These are the material minimums and maximums.
    public float average = 1;

    //These are possible rewards.
    public Equipment item;

    //Grant the reward.
    public string GrantReward()
    {
        switch (rewardType)
        {
            case RewardType.Nothing:
                return "Got Nothing!";
            case RewardType.Item:
                return GrantItem();
            case RewardType.BasicMaterial:
                return GrantMats("basic", ref Player.basicMaterial);
            case RewardType.AdvancedMaterial:
                return GrantMats("advanced", ref Player.advancedMaterial);
            case RewardType.RainbowMaterial:
                return GrantMats("rainbow", ref Player.rainbowMaterial);
            case RewardType.LuminousMaterial:
                return GrantMats("luminous", ref Player.luminousMaterial);
            case RewardType.LightShadowMaterial:
                return GrantMats("light and shadow", ref Player.lightShadowMaterial);
            case RewardType.Weapon:
                return GrantItem(EquipType.Weapon);
            case RewardType.Armor:
                return GrantItem(EquipType.Armor);
            case RewardType.Accessory:
                return GrantItem(EquipType.Accessory);
        }
        return null;
    }

    //Grant an item
    public string GrantItem()
    {
        Equipment e = Equipment.GetItem(item);
        return GrantItem(e);
    }
    public string GrantItem(EquipType et)
    {
        Equipment e = Equipment.GetItem(item, et);
        return GrantItem(e);
    }
    public string GrantItem(Equipment e)
    {
        Player.playerEquips.Add(e);
        return "Got " + e.equipType + "!";
    }

    //Grant basic mats.
    public string GrantMats(string name, ref int mat)
    {
        int r = Mathf.CeilToInt(Random.Range(average * 0.85f, average * 1.15f));
        mat += r;
        return string.Format("Got {0} summoning materials!", name);
    }
}
