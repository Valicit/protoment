using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    //This is an inheritable class for creating status effects that can be put on characters.

    //This is the Status Name.
    public string statusName;

    //This is the icon the status effect shows when it's on the character.
    public Sprite statusIcon;

    //this is basic information.
    public bool isStackable;
    public bool isNegative;
    public int duration;
    public bool preventAction;
    public float percentDamage;
    public float percentHealing;
    public float HPmod = 1;
    public float STRmod = 1;
    public float DEFmod = 1;
    public float INTmod = 1;
    public float DEXmod = 1;
    public float AGImod = 1;
    public float critMod = 1;
    public float critDMGMod = 1;
    public float speedMod = 1;
    public float takenDamageMod;
}
