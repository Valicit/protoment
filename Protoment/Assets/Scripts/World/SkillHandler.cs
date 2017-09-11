using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillHandler
{
    //This class handles all the skill uses through the game. It accepts a Skill as an argument, and pieces together the data inside that skill to actually do something.

    //This is a reference to the last units that were targetted, for skills that repeat.
    public static List<Unit> repeatTargets;

    //This is how long the battle needs to be paused for the skill.
    public static float busy = 0;

    #region Skill Conditions
    //Get which skill should be used.
    public static Skill ChooseSkill(Unit attacker, Party ally, Party enemy)
    {
        //For each skill, starting with the third.
        for (int sId = attacker.mySkills.Length - 1; sId >= 0; sId--)
        {
            //If this skill is off cooldown.
            if (attacker.mySkills[sId].GetCD() <= 0 && !attacker.mySkills[sId].isPassive)
            {
                //If the skill meets its conditions.
                if (ChooseCondition(attacker, ally, enemy, attacker.mySkills[sId]))
                {
                    return attacker.mySkills[sId];
                }
            }
        }

        //Default to the first skill.
        return attacker.mySkills[0];
    }

    //Returns true if the skill condition is met.
    public static bool ChooseCondition(Unit attacker, Party ally, Party enemy, Skill s)
    {
        //Based on the condition.
        switch (s.condition)
        {
            case UseCondition.None:
                return true;
            case UseCondition.UnderPercentHP:
                if (GetTargets(attacker, s, ally, enemy, 0).Find(n => (float)n.cHP / (float)n.GetmHP() <= s.conditionValue) != null) return true;
                else return false;
        }

        //Default to false.
        return false;
    }
    #endregion

    #region Skill Components
    //Use a skill!
    public static void UseSkillAuto(Unit attacker, Skill s, Party allyParty, Party enemyParty)
    {
        //Set us as busy for the duration.
        busy = s.pauseDuration;

        //Announce the skill we used.
        attacker.textQueue.Add(s.displayName);
        attacker.textColor.Add(Color.yellow);

        //Put the skill on cd.
        s.SetCD();

        //For however many skill components there are.
        for (int component = 0; component < s.components.Length; component++)
        {
            //Find targets.
            List<Unit> targets = GetTargets(attacker, s, allyParty, enemyParty, component);

            //Based on which component we have, do a thing.
            switch (s.components[component]) //Note to self, replace this with a dictionary at some point.
            {
                case SkillComponent.True:
                    TrueHit(attacker, targets, s, component);
                    break;
                case SkillComponent.NormalHit:
                    NormalHit(attacker, targets, s, component);
                    break;
                case SkillComponent.NormalHitChance:
                    NormalHitChance(attacker, targets, s, component);
                    break;
                case SkillComponent.RemoveNegativeEffect:
                    RemoveStatus(attacker, targets, s, component, true);
                    break;
                case SkillComponent.RemovePositiveEffect:
                    RemoveStatus(attacker, targets, s, component, false);
                    break;
                case SkillComponent.ModATB:
                    ModATB(attacker, targets, s, component);
                    break;

            }

            //Do any work with status effects that needs to be done.
            HandleStatusEffects(s, targets, component);

            //For each target, set counter references, if this wasn't a counter attack.
            if (!s.isCounter)
            {
                foreach (Unit u in targets)
                {
                    u.lastAttacker = attacker;
                    u.lastAttackerParty = allyParty;
                    if (!s.targetAlly[component]) u.lastAllyParty = enemyParty;
                    else u.lastAllyParty = allyParty;
                }
            }
        }
    }

    //This is what happens when you use a skill manually.
    public static void UseSkillManual(Unit attacker, Skill s, Party allyParty, Party enemyParty, Unit targeted)
    {
        //Set us as busy for the duration.
        busy = s.pauseDuration;

        //Announce the skill we used.
        attacker.textQueue.Add(s.displayName);
        attacker.textColor.Add(Color.yellow);

        //Put the skill on cd.
        s.SetCD();

        //For however many skill components there are.
        for (int component = 0; component < s.components.Length; component++)
        {
            //Find targets.
            List<Unit> targets = new List<Unit>();
            if (component > 0) targets = GetTargets(attacker, s, allyParty, enemyParty, component);
            else targets = GetTargetsManual(attacker, s, allyParty, enemyParty, targeted);

            //Based on which component we have, do a thing.
            switch (s.components[component]) //Note to self, replace this with a dictionary at some point.
            {
                case SkillComponent.True:
                    TrueHit(attacker, targets, s, component);
                    break;
                case SkillComponent.NormalHit:
                    NormalHit(attacker, targets, s, component);
                    break;
                case SkillComponent.NormalHitChance:
                    NormalHitChance(attacker, targets, s, component);
                    break;
                case SkillComponent.RemoveNegativeEffect:
                    RemoveStatus(attacker, targets, s, component, true);
                    break;
                case SkillComponent.RemovePositiveEffect:
                    RemoveStatus(attacker, targets, s, component, false);
                    break;
                case SkillComponent.ModATB:
                    ModATB(attacker, targets, s, component);
                    break;

            }

            //Do any work with status effects that needs to be done.
            HandleStatusEffects(s, targets, component);

            //For each target, set counter references, if this wasn't a counter attack.
            if (!s.isCounter)
            {
                foreach (Unit u in targets)
                {
                    u.lastAttacker = attacker;
                    u.lastAttackerParty = allyParty;
                    if (!s.targetAlly[component]) u.lastAllyParty = enemyParty;
                    else u.lastAllyParty = allyParty;
                }
            }
        }
    }

    //This is for handling status effects.
    private static void HandleStatusEffects(Skill s, List<Unit> targets, int component)
    {
        //Roll for each target we just hit.
        foreach (Unit u in repeatTargets)
        {
            //If we roll properly to put it on.
            if (Random.Range(0, 100) <= s.statusChance[component] && s.statusEffect[component] != null)
            {
                //Get our status.
                StatusEffect sf = StatusEffect.Instantiate(s.statusEffect[component]);
                sf.duration = s.statusDuration[component];
                //Add it!
                u.AddStatusEffect(sf);
                u.textQueue.Add(s.statusEffect[component].statusName);
                u.textColor.Add(Color.red);
            }
        }
    }


    //This is a true hit. It can not be blocked, dodged, or defended against in any way. It does not roll for crit. Used almost exclusively for healing.
    public static void TrueHit(Unit attacker, List<Unit> targets, Skill s, int component)
    {
        //for each target.
        for (int z = 0; z < targets.Count; z++)
        {
            targets[z].TakeHit((long)(attacker.GetStat(s.atkStat) * s.value1[z]), s.isHealing, 1f);
        }
    }

    //This is a normal hit, with no bells and whistles.
    public static void NormalHit(Unit attacker, List<Unit> targets, Skill s, int component)
    {
        //For each target.
        for (int z = 0; z < targets.Count; z++)
        {
            //If its healing, do healing. Otherwise do damage.
            targets[z].TakeHit(MathP.GetDamage((long)(attacker.GetStat(s.atkStat) * s.value1[z]), targets[z].GetStat(s.defStat)), s.isHealing, MathP.GetCritDamage(attacker, s.critOffset[component]) + s.critDMGOffset[component]);
        }
    }

    //This is a normal hit with a chance to go off.
    public static void NormalHitChance(Unit attacker, List<Unit> targets, Skill s, int component)
    {
        //Roll odds.
        if (Random.Range(0, 100) <= s.value2[component]) NormalHit(attacker, targets, s, component);
    }

    //This removes value1 status effects at a value2 chance.
    public static void RemoveStatus(Unit attacker, List<Unit> targets, Skill s, int component, bool isNegative)
    {
        
        //For each target.
        for (int t = 0; t < targets.Count; t++)
        {
            //Roll odds
            if (Random.Range(0, 100) <= s.value2[component])
            {
                //Get a list of negative effects.
                List<StatusEffect> r = targets[t].myStatusEffects.FindAll(n => n.isNegative == isNegative);
                
                //For each effect we need to remove.
                for (int e = 0; e < s.value1[component]; e++)
                {
                    //If there is a status effect left to remove, remove it.
                    if (r.Count > 0)
                    {
                        targets[t].myStatusEffects.Remove(r[0]);
                        r.RemoveAt(0);
                    }
                }
            }
        }
    }

    //This modifies the ATB gauge without doing damage. It modifies it by value1 at a value2 chance.
    public static void ModATB(Unit attacker, List<Unit> targets, Skill s, int component)
    {
        //For each target,
        for (int t = 0; t < targets.Count; t++)
        {
            //Roll odds
            if (Random.Range(0, 100) <= s.value2[component])
            {
                targets[t].atb += s.value1[component];
            }
        }
    }
    #endregion


    #region Targeting
    //Get a target based on the skills target type.
    public static List<Unit> GetTargets(Unit attacker, Skill s, Party allyParty, Party enemyParty, int component)
    {
        //Figure out which party to look at.
        Party tParty;
        if (s.targetAlly[component]) tParty = allyParty;
        else tParty = enemyParty;

        //This holds our result.
        List<Unit> r = new List<Unit>();

        //Based on the target area, return targets.
        switch (s.targetArea[component])
        {
            case TargetArea.SingleRandom:
                r = GetSingleRandom(tParty);
                break;
            case TargetArea.All:
                r = tParty.GetAllLiving();
                break;
            case TargetArea.Self:
                r = new List<Unit>() { attacker };
                break;
            case TargetArea.Repeat:
                r = repeatTargets;
                break;
        }
        repeatTargets = r;
        return r;
    }

    //Get targets for manual mode.
    public static List<Unit> GetTargetsManual(Unit attacker, Skill s, Party ally, Party enemy, Unit targeted)
    {
        //Figure out which party to look at.
        Party tParty;
        if (targeted.isAlly) tParty = ally;
        else tParty = enemy;

        //This holds our result.
        List<Unit> r = new List<Unit>();

        //Based on how we target, do a thing.
        switch (s.manualTargetType)
        {
            case ManualTargetType.Single:
                r.Add(targeted);
                break;
            case ManualTargetType.Team:
                r.AddRange(tParty.GetAllUnits());
                break;
        }

        //If the only valid target is self, do that instead.
        if (s.targetArea[0] == TargetArea.Self) r = new List<Unit>() { attacker };

        //Return the result.
        repeatTargets = r;
        return r;
    }


    //This is random targeting. It picks a valid target at random.
    public static List<Unit> GetSingleRandom(Party targetParty)
    {
        return new List<Unit> { targetParty.GetAllLiving()[Random.Range((int)0, targetParty.GetAllLiving().Count)] };
    }
    #endregion
}
