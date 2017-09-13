using UnityEngine;
using System.Collections;

//This is for element selection.
public enum Element
{
    Fire,
    Water,
    Wood,
    Electric,
    Light,
    Dark
}

//This is for rarity.
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Unit")]
public class UnitData : ScriptableObject
{
    //This contains all of the basic information about what each unit is. The base stats and values for a class.

    //This is unit information.
    public string uName;
    public string job;
    public Element uElement;
    public Rarity uRarity;

    //These are stats.
    public int HP;
    public int STR;
    public int DEF;
    public int INT;
    public int SPR;
    public int DEX;
    public int AGI;
    public float crit;
    public float critDMG;
    public float speed;

    //These are the units skills.
    public string skill1;
    public string skill2;
    public string skill3;

    //This is the sprite for the character.
    public Sprite unitSprite;
}
