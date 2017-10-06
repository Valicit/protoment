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
    Speed,
    Flat,
    CurrentHP,
    MissingHP,
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
    NormalAttackAddedCrit,
    NormalAttackLeech,
    NormalAttackPartyLeech,
    NormalAttackWithStatusDamage,
    NormalAttackWithBuffDamage,
    ApplyStatusVariableValue,
    StealStatus,
    StealBuff,
    GiveStatus,
    GiveBuff,
    HealPercent,
    StealHPPercent,
}

//This controls what kind of targets are found and hit by the enum.
public enum SkillTargets
{
    SingleRandom,
    SingleFrontLine,
    SingleBackLine, //Not Implemented.
    RowRandom,
    RowFront, //Not Implemented.
    RowBack, //Not Implemented.
    LineRandom, //Not Implemented.
    LineFront,
    LineBack, //Not Implemented.
    Self,
    Repeat,
    All,
    SingleLowest
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
    public bool excludeSelf;
    private float hitRoll;
    private const float ELEMPLUS = 1.5f;
    private const float ELEMMINUS = 0.6667f;


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

            case SkillTargets.SingleBackLine:
                if (data.selectedUnit != null) targets.Add(data.selectedUnit);
                else if (p.GetSingleFrontLine() != null) targets.Add(p.GetSingleBackLine());
                break;

            case SkillTargets.RowRandom:
                if (data.selectedUnit != null) targets.AddRange(p.GetRowContaining(data.selectedUnit));
                else if (p.GetSingleRandom() != null) targets.AddRange(p.GetRowContaining(p.GetSingleRandom()));
                break;

            case SkillTargets.LineRandom:
                if (data.selectedUnit != null) targets.AddRange(p.GetLineContaining(data.selectedUnit));
                else if (p.GetSingleRandom() != null) targets.AddRange(p.GetLineContaining(p.GetSingleRandom()));
                break;

            case SkillTargets.LineFront:
                if (data.selectedUnit != null) targets.AddRange(p.GetLineContaining(data.selectedUnit));
                else targets.AddRange(p.GetFrontLine());
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

            case SkillTargets.SingleLowest:
                if (data.selectedUnit != null) targets.Add(data.selectedUnit);
                else if (p.GetLowestHP() != null) targets.Add(p.GetLowestHP());
                break;
        }

        //return the result.
        data.repeatTargets = targets;
        if (excludeSelf) targets.Remove(data.actor);
        return targets;
    }

    //This gets the main damage stat of the attack.
    public float GetSkillDamage(Unit actor)
    {
        Unit u = actor;
        float r = 0;
        for (int i = 0; i < statValues.Length; i++)
        {
            r += (float)(u.GetStat(statValues[i].stat) * statValues[i].ratio);
        }
        return r;
    }

    //Make a single attack.
    public long AttackTarget(Unit actor, Unit target, AttackData data, bool isCrit, float dmgMod)
    {
        //If we crit, apply that.
        float critmod = 1f;
        if (isCrit) critmod = actor.GetCritDMG();

        //Apply elemental advantage.
        float hit = hitRoll;
        if (MathP.IsElementAdavantage(actor.uElement, target.uElement) == 1) hit /= ELEMPLUS;
        else if (MathP.IsElementAdavantage(actor.uElement, target.uElement) == -1) hit /= ELEMMINUS;

        //Check if we miss.
        if (hit < ((float)actor.GetDEX() / (float)target.GetAGI()) * 100)
        {
            //Roll for damage.
            long d = MathP.GetDamage((decimal)(GetSkillDamage(actor) * dmgMod), target.GetStat(defenseStat));

            //Apply elemental advantage.
            if (MathP.IsElementAdavantage(actor.uElement, target.uElement) == 1) d = (long)(d * ELEMPLUS);
            else if (MathP.IsElementAdavantage(actor.uElement, target.uElement) == -1) d = (long)(d * ELEMMINUS);

            //Deal the damage.
            if (target.myStatusEffects.Find(n => n.isCover) == null)
            {
                target.TakeHit(d, false, critmod, MathP.IsElementAdavantage(actor.uElement, target.uElement));
            }
            else
            {
                //Get how much damage is diverted.
                for (int i = 0; i < target.myStatusEffects.Count; i++)
                {
                    long diverted = 0;
                    if (target.myStatusEffects[i].isCover && target.myStatusEffects[i].applier != null)
                    {
                        diverted += (long)((target.myStatusEffects[i].variableValue / 100) * (float)d);
                        Debug.Log("Yes");
                        //If the diverter is still alive and stuff.
                        if (target.myStatusEffects[i].applier.IsAlive())
                        {
                            target.myStatusEffects[i].applier.TakeHit(diverted, false, 1f);
                            d -= diverted;
                        }
                    }
                }

                //Finally, deal remaining damage to the target.
                target.TakeHit(d, false, critmod, MathP.IsElementAdavantage(actor.uElement, target.uElement));

            }

            //Take any thorns damage.
            if(target.GetThorns(Mathf.CeilToInt((long)(d * critmod))) > 0) actor.TakeHit(target.GetThorns(Mathf.CeilToInt((long)(d * critmod))), false, 1f);
            return d;
        }
        else
        {
            target.TakeMiss();
            return -1;
        }
    }
    public long AttackTarget(Unit actor, Unit target, AttackData data)
    {
        //Roll for a crit.
        bool crit = false;
        if (Random.Range(0, 100) < actor.GetCrit()) crit = true;

        //Attack the target.
        return AttackTarget(actor, target, data, crit, 1f);
    }
    public long AttackTarget(Unit actor, Unit target, AttackData data, float dmgMod)
    {
        //Roll for a crit.
        bool crit = false;
        if (Random.Range(0, 100) < actor.GetCrit()) crit = true;

        //Attack the target.
        return AttackTarget(actor, target, data, crit, dmgMod);
    }

    //Apply status effects.
    public void ApplyStatusEffects(Unit t, AttackData data, float variableValue)
    {
        //For each status effect.
        for (int se = 0; se < statusEffects.Length; se++)
        {
            //Apply elemental advantage.
            float hit = hitRoll;
            if (MathP.IsElementAdavantage(data.actor.uElement, t.uElement) == 1) hit /= ELEMPLUS;
            else if (MathP.IsElementAdavantage(data.actor.uElement, t.uElement) == -1) hit /= ELEMMINUS;

            //Roll.
            float rand = Random.Range(0, 100);
            if ((rand < statusChances[se] && (hit < ((float)data.actor.GetDEX() / (float)t.GetAGI()) * 100) || !statusEffects[se].isNegative))
            {
                StatusEffect r = StatusEffect.Instantiate(statusEffects[se]);
                if (t == data.actor) r.duration = statusDurations[se] + 1;
                else r.duration = statusDurations[se];
                r.applier = data.actor;
                r.variableValue = variableValue;
                t.AddStatusEffect(r);
            }
            else if (rand < statusChances[se] && statusEffects[se].isNegative && hit > ((float)data.actor.GetDEX() / (float)t.GetAGI()) * 100)
            {
                t.textQueue.Add("Resist");
                t.textColor.Add(Color.white);
            }
        }
    }
    public void ApplyStatusEffects(Unit t, AttackData data) { ApplyStatusEffects(t, data, 0); }

    //Give status effects to someone.
    public void MoveStatusEffect(Unit giver, List<Unit> recievers, AttackData data, StatusEffect effect)
    {
        //For each target that gets it.
        foreach (Unit u in recievers)
        {
            u.AddStatusEffect(StatusEffect.Instantiate(effect));
        }

        //Remove that effect from the giver.
        giver.myStatusEffects.Remove(effect);
    }
    #endregion

    #region Component execution stuff.
    //Execute the component.
    public void UseComponent(AttackData data)
    {
        //Make the hit roll.
        hitRoll = Random.Range(0f, 100f);

        //Based on what kind of component this is.
        switch (behaviour)
        {
            case SkillBehaviour.None:
                break;

            case SkillBehaviour.Healing:
                Healing(GetTargets(data), data);
                break;

            case SkillBehaviour.HealPercent:
                HealPercent(GetTargets(data), data);
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

            case SkillBehaviour.NormalAttackLeech:
                NormalAttackLeech(GetTargets(data), data);
                break;

            case SkillBehaviour.NormalAttackPartyLeech:
                NormalAttackPartyLeech(GetTargets(data), data);
                break;

            case SkillBehaviour.NormalAttackWithStatusDamage:
                NormalAttackStatusBasedDamage(GetTargets(data), data, true);
                break;

            case SkillBehaviour.NormalAttackWithBuffDamage:
                NormalAttackStatusBasedDamage(GetTargets(data), data, false);
                break;

            case SkillBehaviour.ApplyStatusVariableValue:
                ApplyStatusWithVariable(GetTargets(data), data);
                break;

            case SkillBehaviour.StealStatus:
                StealStatusEffects(GetTargets(data), data, true);
                break;

            case SkillBehaviour.StealBuff:
                StealStatusEffects(GetTargets(data), data, false);
                break;

            case SkillBehaviour.GiveStatus:
                GiveStatusEffects(GetTargets(data), data, true);
                break;

            case SkillBehaviour.GiveBuff:
                GiveStatusEffects(GetTargets(data), data, false);
                break;

            case SkillBehaviour.StealHPPercent:
                StealHP(GetTargets(data), data);
                break;
        }
    }

    //Heal the targets.
    public void Healing(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            u.TakeHit((long)(GetSkillDamage(data.actor) * data.actor.GetEquipStatsPercentage(EquipStats.Healing)), true, 1f);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }

    //Heal the targets for a flat % of their life.
    public void HealPercent(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            u.TakeHit((long)((GetSkillDamage(data.actor) * u.GetmHP()) * data.actor.GetEquipStatsPercentage(EquipStats.Healing)), true, 1f);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.helpedTargets, targets);
    }

    //Deals direct and true damage.
    public void TrueDamage(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            u.TakeHit((long)GetSkillDamage(data.actor), false, 1f);
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
            ApplyStatusEffects(u, data);
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
            ApplyStatusEffects(u, data);
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
                u.atb = Mathf.Clamp(u.atb + (otherValues[0] * 10), 0, 1000);
                
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
            AttackTarget(data.actor, u, data, crit, 1f);
            ApplyStatusEffects(u, data);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //Make a normal attack and absorb othervalues[0]% life.
    public void NormalAttackLeech(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            long d =AttackTarget(data.actor, u, data);
            ApplyStatusEffects(u, data);

            //Heal the actor for a portion of the damage dealt.
            data.actor.TakeHit(Mathf.CeilToInt((float)d * (otherValues[0] / 100)), true, 1f);
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //Make a normal attack that absorbs othervalues[0]% life for the whole party.
    public void NormalAttackPartyLeech(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            long d = AttackTarget(data.actor, u, data);
            ApplyStatusEffects(u, data);

            //For each party member.
            foreach (Unit pm in data.actorParty.GetAllLiving())
            {
                //Heal the actor for a portion of the damage dealt.
                pm.TakeHit(Mathf.CeilToInt((float)d * (otherValues[0] / 100)), true, 1f);
            }
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //Make an attack that deals othervalues[0]% extra damage for each status on the character.
    public void NormalAttackStatusBasedDamage(List<Unit> targets, AttackData data, bool isNegative)
    {
        //For each unit we're attacking.
        foreach (Unit u in targets)
        {
            //Get damage mod.
            float dmgMod = 1 + ((otherValues[0] * u.myStatusEffects.FindAll(n => n.isNegative == isNegative).Count) / 100);

            //Attack them and apply status effects.
            AttackTarget(data.actor, u, data, dmgMod);
            ApplyStatusEffects(u, data);
        }
        //Record what happened.
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //Apply a status that has a variable value, like thorns or a shield. The value is specified in the stat values section.
    public void ApplyStatusWithVariable(List<Unit> targets, AttackData data)
    {
        foreach (Unit u in targets)
        {
            ApplyStatusEffects(u, data, GetSkillDamage(data.actor));
        }
        data.AddAffectedUnits(data.affectedTargets, targets);
    }

    //Steal status effects from enemies.
    public void StealStatusEffects(List<Unit> targets, AttackData data, bool isNegative)
    {
        //For each target we are stealing from.
        foreach (Unit u in targets)
        {
            //Get however many times we are doing this.
            int count = u.myStatusEffects.FindAll(n => n.isNegative == isNegative).Count;
            if (otherValues.Length > 0) count = Mathf.Min(count, Mathf.FloorToInt(otherValues[0]));

            //For however many status effects we are doing this for,.
            for (int i = 0; i < count; i++)
            {
                MoveStatusEffect(u, new List<Unit>() { data.actor }, data, u.myStatusEffects.FindAll(n => n.isNegative == isNegative)[i]);
            }
        }
    }

    //Give over othervalues[0] status effects to a list of targets.
    public void GiveStatusEffects(List<Unit> targets, AttackData data, bool isNegative)
    {
        //Get however many times we are doing this.
        int count = data.actor.myStatusEffects.FindAll(n => n.isNegative == isNegative).Count;
        if (otherValues.Length > 0) count = Mathf.Min(count, Mathf.FloorToInt(otherValues[0]));

        //For however many status effects we are doing this for,.
        for (int i = 0; i < count; i++)
        {
            MoveStatusEffect(data.actor, targets, data, data.actor.myStatusEffects.FindAll(n => n.isNegative == isNegative)[i]);
        }
    }

    //Steal a percentage of an enemies HP directly.
    public void StealHP(List<Unit> targets, AttackData data)
    {
        //This is how much health we stole.
        long stolen = 0;

        //For each target.
        foreach (Unit u in targets)
        {
            //Get the HP we're stealing.
            long d =  Mathf.CeilToInt(u.GetmHP() * GetSkillDamage(data.actor));
            stolen += (long)Mathf.Min(d, u.cHP);
            u.TakeHit(d, false, 1f);
        }

        //Restore the HP.
        data.actor.TakeHit(stolen, true, 1f);
    }
    #endregion
}
