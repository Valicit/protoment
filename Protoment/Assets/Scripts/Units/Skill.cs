using UnityEngine;
using System.Collections;

//This controls how the skill targets.
public enum TargetArea
{
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
    Repeat
}

public enum SkillComponent
{
    NormalHit,
    NormalHitChance
}

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    //This class has all of the values that control how a skill works.

    //These are basic skill values.
    public string displayName;
    public bool isPassive;
    public SkillComponent[] components;
    public bool[] targetAlly;
    public TargetArea[] targetArea;
    public StatusEffect[] statusEffect;
    public float[] statusChance;
    public float[] value1;
    public float[] value2;
    public float pauseDuration;

    //Create a copied instance of this skill, for passing out to individual units.
    public static Skill GetSkill(Skill s)
    {
        return (Skill)s.MemberwiseClone();
    }
}
