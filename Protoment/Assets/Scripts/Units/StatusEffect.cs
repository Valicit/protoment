﻿using UnityEngine;
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
    public bool showIcon = true;
    public bool isStackable;
    public bool isNegative;
    public bool permanent;
    public int duration;
    public bool preventAction;
    public bool provoke;
    public bool isThorns;
    public bool isShield;
    public bool isCover;
    public int provokeSkill = 0;
    public bool invulnerable;
    public bool removeOnDamage;
    public float variableValue;
    public float percentDamage;
    public float percentHealing;
    public float HPmod = 1;
    public float STRmod = 1;
    public float DEFmod = 1;
    public float INTmod = 1;
    public float SPRmod = 1;
    public float DEXmod = 1;
    public float AGImod = 1;
    public float critMod = 1;
    public float critDMGMod = 1;
    public float speedMod = 1;
    public float takenDamageMod;
    public StatusEffect[] counterStatus;
    public float[] counterStatusChance;
    public int[] counterStatusDuration;
    public Unit applier;

    //This is for passives that trigger other skills.
    public Skill triggerSkill;
    public bool triggerTurnEnd;
    public bool triggerOnDeath;
    public bool triggerOnGetCrit;
}
