using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

//This helps decide what is and is not a valid target.
public enum ValidTarget
{
    SingleEnemy,
    ClosestEnemy
}

public class Skill
{
    //This class has all of the methods a skill needs to work.

    //These are a list of variables commonly needed by skills.
    public string displayName = "Skill";
    public ValidTarget validTarget = ValidTarget.SingleEnemy;
    public int cd = 0;
    internal int maxCD = 0;
    public bool isCounterable = true;
    public bool isPassive = false;

    //Skill use requires attack data, populated by all the data that can be collected before the skill starts.
    public virtual void UseSkill(AttackData data) { }

    //These methods are all used when things happen in the scene.
    public virtual void OnGetHurt(AttackData data) { }
    public virtual void OnGetHealed(AttackData data) { }
    public virtual void OnUseSkill(AttackData data) { }
    public virtual void OnDeath(AttackData data) { }
    public virtual void OnTurnStart(AttackData data) { }
    public virtual void OnTurnEnd(AttackData data) { }
    public virtual void OnAllyHurt(AttackData data) { }
    public virtual void OnAllyHealed(AttackData data) { }
    public virtual void OnAllyTurn(AttackData data) { }
    public virtual void OnAllyDeath(AttackData data) { }

    //This checks if the skill is ready to use.
    public bool IsReady()
    {
        if (cd == 0 && !isPassive) return true;
        else return false;
    }

    //Count down the cooldown by one.
    public void CountCD()
    {
        cd = Mathf.Max(0, cd--);
    }

    //Get whether the attack is being made against a valid target.
    public bool IsValidTarget(Unit target, AttackData data)
    {
        switch (validTarget)
        {
            case ValidTarget.SingleEnemy:
                if (data.defendingParty.ValidateSingleRandom().Contains(target)) return true;
                break;
            case ValidTarget.ClosestEnemy:
                if(data.defendingParty.ValidateSingleFrontLine().Contains(target)) return true;
                break;
        }

        //Default to false.
        return false;
    }
}
