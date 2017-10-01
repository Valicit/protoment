using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This says what stats are used to attack and defend with.
public enum StatBase
{
    None,
    HP,
    STR,
    DEF,
    INT,
    SPR,
    DEX,
    AGI,
    Speed
}

//This controls what main code the skill component executes. What it DOES to the target, without anything being added.
public enum SkillBehaviour
{
    None,
    Healing, 
    TrueDamage,
    AddStatus,
    AddBuff,
    RemoveStatus,
    RemoveBuff,
    ModAttackBar,
    NormalAttack,
    NormalAttackWithChance,
    NormalAttackAddedCrit
}

//This controls what kind of targets are found and hit by the enum.
public enum SkillTargets
{
    SingleRandom,
    SingleFrontLine,
    SingleBackLine, //Not Implemented.
    RowRandom, //Not Implemented.
    RowFront, //Not Implemented.
    RowBack, //Not Implemented.
    LineRandom, //Not Implemented.
    LineFront, //Not Implemented.
    LineBack, //Not Implemented.
    Self,
    Repeat,
    All
}

[System.Serializable]
public class SkillComponent
{
    //A skill is made up of a bunch of these. It has all the bits needed to tell the skill what to do at one stage of itself. Does nothing for passive skills.

    //This is the main skill behaviour.
    public SkillBehaviour behaviour;
    public SkillTargets skillTargets;

    //These are the stats that the skill runs off of.
    public StatBase defenseStat;
    public StatRatioPair[] statValues;
    public float[] otherValues;

    //This is a list of status effects that are applied, if any.
    public StatusEffect[] statusEffects;
    public float[] statusChances;
    public int[] statusDurations;

    //These are final values.
    public bool targetAlly;

    #region Component Components.
    //This gets all the targets hit by the component.
    public List<Unit> GetTargets(AttackData data)
    {
        //Figure out which party we are targeting.
        Party p = data.defendingParty;
        if (targetAlly) p = data.actorParty;

        //Create a list to hold results.
        List<Unit> targets = new List<Unit>();

        //Based on the skill targets, return targets.
        switch (skillTargets)
        {
            case SkillTargets.SingleRandom:
                if (data.selectedUnit != null) targets.Add(data.selectedUnit);
                else if(p.GetSingleRandom() != null) targets.Add(p.GetSingleRandom());
                break;

            case SkillTargets.SingleFrontLine:
                if (data.selectedUnit != null) targets.Add(data.selectedUnit);
                else if (p.GetSingleFrontLine() != null) targets.Add(p.GetSingleFrontLine());
                break;

            case SkillTargets.RowRandom:
                if (data.selectedUnit != null) targets.AddRange(p.GetUnitRow(data.selectedUnit));
                else if (p.GetSingleRandom() != null) targets.AddRange(p.GetUnitRow(p.GetSingleRandom()));
                break;

            case SkillTargets.Self:
                targets.Add(data.actor);
                break;

            case SkillTargets.Repeat:
                targets.AddRange(data.repeatTargets);
                break;

            case SkillTargets.All:
                targets.AddRange(p.GetAllLiving());
                break;
        }

        //return the result.
        data.repeatTargets = targets;
        return targets;
    }

    //This gets the main damage stat of the attack.
    public long GetSkillDamage(Unit actor)
    {
        Unit u = actor;
        long r = 0;
        for (int i = 0; i < statValues.Length; i++)
        {
            r += (long)(u.GetStat(statValues[i].stat) * statValues[i].ratio);
        }
        return r;
    }

    //Make a single attack.
    public void AttackTarget(Unit actor, Unit target, AttackData data, bool isCrit)
    {
        //If we crit, apply that.
        float critmod = 1f;
        if (isCrit) critmod = actor.GetCritDMG();

        //Check if we miss.
        if (Random.Range(0, 100) < ((float)actor.GetDEX() / (float)target.GetAGI()) * 100)
        {
            //Deal the damage.
            target.TakeHit(MathP.GetDamage(GetSkillDamage(actor), target.GetStat(defenseStat)), false, critmod);
        }
        else
        {
            target.TakeMiss();
        }
    }
    public void AttackTarget(Unit actor, Unit target, AttackData data)
    {
        //Roll for a crit.
        bool crit = false;
        if (Random.Range(0, 100) < actor.GetCrit()) crit = true;

        //Attack the target.
        AttackTarget(actor, target, data, crit);
    }

    //Apply status effects.
    public void ApplyStatusEffects(Unit t, AttackData data)
    {
        //For each status effect.
        for (int se = 0; se < statusEffects.Length; se++)
        {
            //Roll.
            if (Random.Range(0, 100) < statusChances[se])
            {
                StatusEffect r = StatusEffect.Instantiate(statusEffects[se]);
                if (t == data.actor) r.duration = statusDurations[se] + 1;
                else r.duration = statusDurations[se];
                t.AddStatusEffect(r);
            }
        }
    }
    #endregion

    #region Component execution stuff.
    //Execute the component.
    public void UseComponent(AttackData data)
    {
        //Based on what kind of component this is.
        switch (behaviour)
        {
            case SkillBehaviour.None:
                break;

            case SkillBehaviour.Healing:
                Healing(GetTargets(data), data);
                break;

            case SkillBehaviour.TrueDamage:
                TrueDamage(GetTargets(data), data);
                break;

            case SkillBehaviour.AddStatus:
                AddStatus(GetTargets(data), data);
                break;

            case SkillBehaviour.AddBuff:
                AddBuff(GetTargets(data), data);
                break;

            case SkillBehaviour.RemoveStatus:
                RemoveStatus(GetTargets(data), data);
                break;

            case SkillBehaviour.RemoveBuff:
                RemoveBuff(GetTargets(data), data);
                break;

            case SkillBehaviour.ModAttackBar:
                ModifyAttackBar(GetTargets(data), data);
                break;

            case SkillBehaviour.NormalAttack:
                NormalAttack(GetTargets(data), data);
                break;

            case SkillBehaviour.NormalAttackWithChance:
                NormalAttackWithChance(GetTargets(data), data);
                break;

            case SkillBehaviour.NormalAttackAddedCrit:
                NormalAttackAddedCrit(GetTargets(data), data);
                break;
        }
    }

    //Heal the targets.
    public void Healing(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            u.TakeHit(GetSkillDamage(data.actor), true, 1f);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }

    //Deals direct and true damage.
    public void TrueDamage(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            u.TakeHit(GetSkillDamage(data.actor), false, 1f);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }
    
    //Add a status effect to the targets.
    public void AddStatus(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }
    
    //Add buffs to the targets.
    public void AddBuff(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }

    //Remove status effects on the target.
    public void RemoveStatus(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            //Get how many effects to remove.
            int i = 0;
            //If we have no number of effects to remove.
            if (otherValues.Length == 0) i = u.myStatusEffects.FindAll(n => n.isNegative).Count;
            else i = (int)Mathf.Min(otherValues[0], u.myStatusEffects.FindAll(n => n.isNegative).Count);

            //For each status effect we get to remove.
            for (int x = 0; x < i; x++)
            {
                u.myStatusEffects.Remove(u.myStatusEffects.FindAll(n => n.isNegative)[0]);
            }
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }
    public void RemoveBuff(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            //Get how many effects to remove.
            int i = 0;
            //If we have no number of effects to remove.
            if (otherValues.Length == 0) i = u.myStatusEffects.FindAll(n => !n.isNegative).Count;
            else i = (int)Mathf.Min(otherValues[0], u.myStatusEffects.FindAll(n => !n.isNegative).Count);

            //For each status effect we get to remove.
            for (int x = 0; x < i; x++)
            {
                u.myStatusEffects.Remove(u.myStatusEffects.FindAll(n => !n.isNegative)[0]);
            }
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }

    //Modify the attack bar by OtherValues[0] at a otherValues[1] chance.
    public void ModifyAttackBar(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            //roll.
            if (Random.Range(0, 100) < otherValues[1])
            {
                u.atb = Mathf.Clamp(u.atb + otherValues[0], 0, 100);
                
                ApplyStatusEffects(u, data);

                if (statValues.Length > 0) AttackTarget(data.actor, u, data);
            }
        }
    }

    //This is just a normal attack against the target.
    public void NormalAttack(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            AttackTarget(data.actor, u, data);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //This is a normal attack at a otherValues[0] chance.
    public void NormalAttackWithChance(List<Unit> targets, AttackData data)
    {
        //Roll!
        if (Random.Range(0, 100) < otherValues[0])
        {
            //Attack!
            foreach (Unit u in targets)
            {
                AttackTarget(data.actor, u, data);
                ApplyStatusEffects(u, data);
            }
            data.AddAffectedUnits(data.affectedTargets, targets);
        }
    }

    //Makes a normal attack with othervalues[0] added to crit chance.
    public void NormalAttackAddedCrit(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            //Roll for a crit.
            bool crit = false;
            if (data.actor.GetCrit() + otherValues[0] < Random.Range(0, 100)) crit = true;

            //Attack the target.
            AttackTarget(data.actor, u, data, crit);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }
    #endregion
}
