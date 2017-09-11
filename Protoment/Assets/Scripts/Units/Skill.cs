using UnityEngine;
using System.Collections;

//This controls how the skill targets.
public enum TargetArea
{
    None,
    SingleRandom,
    SingleFront,
    SingleBack,
    RandomRow,
    TopRow,
    BottomRow,
    RandomLine,
    FrontLine,
    BackLine,
    Plus,
    X,
    O,
    All,
    Self,
    Repeat
}

//This controls how the skill targets manually.
public enum ManualTargetType
{
    Single,
    Team
}

//These are the bits that decide what kind of hit gets through.
public enum SkillComponent
{
    None,
    True,
    NormalHit,
    NormalHitChance,
    RemoveNegativeEffect,
    RemovePositiveEffect,
    AddStatus,
    ModATB
}

//This says what stats are used to attack and defend with.
public enum StatBase
{
    HP,
    STR,
    DEF,
    INT,
    SPR,
    DEX,
    AGI,
    None
}

//These are use conditions for skills. Skills not meeting the condition will not be used.
public enum UseCondition
{
    None,
    UnderPercentHP
}

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    //This class has all of the values that control how a skill works.

    //These are basic skill values.
    public string displayName;
    public ManualTargetType manualTargetType;
    public UseCondition condition;
    public float conditionValue;
    public bool isPassive;
    public bool isHealing;
    public StatBase atkStat;
    public StatBase defStat;
    public SkillComponent[] components = new SkillComponent[10];
    public bool[] targetAlly = new bool[10];
    public TargetArea[] targetArea = new TargetArea[10];
    public StatusEffect[] statusEffect = new StatusEffect[10];
    public float[] statusChance = new float[10];
    public int[] statusDuration = new int[10];
    public float[] value1 = new float[10];
    public float[] value2 = new float[10];
    public float[] critOffset = new float[10];
    public float[] critDMGOffset = new float[10];
    public float takenDamageMod;
    public Skill counterSkill;
    public bool isCounter;
    public float pauseDuration;

    //Cooldown stuff.
    public int cooldown;
    private int cdCount;
    public int GetCD() { return cdCount; }
    public void SetCD() { cdCount = cooldown; }
    public void CDCountdown() { if(cdCount > 0) cdCount--; }
}
