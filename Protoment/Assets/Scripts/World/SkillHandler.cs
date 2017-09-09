using UnityEngine;
using System.Collections;

public class SkillHandler
{
    //This class handles all the skill uses through the game. It accepts a Skill as an argument, and pieces together the data inside that skill to actually do something.

    //This is a reference to the last units that were targetted, for skills that repeat.
    public static Unit[] repeatTargets;

    //This is how long the battle needs to be paused for the skill.
    public static float busy = 0;

    //Use a skill!
    public static void UseSkill(Unit attacker, Skill s, Party allyParty, Party enemyParty)
    {
        //Set us as busy for the duration.
        busy = s.pauseDuration;

        //Announce the skill we used.
        attacker.textQueue.Add(s.displayName);
        attacker.textColor.Add(Color.yellow);

        //For however many skill components there are.
        for (int i = 0; i < s.components.Length; i++)
        {
            //Based on which component we have, do a thing.
            switch (s.components[i]) //Note to self, replace this with a dictionary at some point.
            {
                case SkillComponent.NormalHit:
                    NormalHit(attacker, GetTargets(s, allyParty, enemyParty, i), s, i);
                    Debug.Log("Normal Hit Triggered");
                    break;
                case SkillComponent.NormalHitChance:
                    NormalHitChance(attacker, GetTargets(s, allyParty, enemyParty, i), s, i);
                    break;
            }

            //If this one causes a status effect.
            if (s.statusEffect[i] != null)
            {
                //Roll for each target we just hit.
                foreach (Unit u in repeatTargets)
                {
                    //If we roll properly to put it on.
                    if (Random.Range(0, 100) <= s.statusChance[i])
                    {
                        u.myStatusEffects.Add(s.statusEffect[i]);
                        u.textQueue.Add(s.statusEffect[i].statusName);
                        u.textColor.Add(Color.red);
                    }
                }
            }
        }
    }

    //Get a target based on the skills target type.
    public static Unit[] GetTargets(Skill s, Party allyParty, Party enemyParty, int i)
    {
        //Set our target party.
        Party tParty;
        if (s.targetAlly[i]) tParty = allyParty;
        else tParty = enemyParty;

        //This holds our result.
        Unit[] r = new Unit[0];

        //Based on the target area, return targets.
        switch (s.targetArea[i])
        {
            case TargetArea.SingleRandom:
                r = GetSingleRandom(s, tParty, i);
                break;
            case TargetArea.Repeat:
                r = repeatTargets;
                break;
        }
        repeatTargets = r;
        return r;
    }

    #region Targeting
    //This is random targeting. It picks a valid target at random.
    public static Unit[] GetSingleRandom(Skill s, Party targetParty, int i)
    {
        return new Unit[1] { targetParty.GetAllLiving()[Random.Range((int)0, targetParty.GetAllLiving().Count)] };
    }
    #endregion

    #region Skill Components
    //This is a normal hit, with no bells and whistles.
    public static void NormalHit(Unit attacker, Unit[] targets, Skill s, int i)
    {
        for (int z = 0; z < targets.Length; z++)
        {
            targets[z].TakeDamage(MathP.GetDamage((long)(attacker.GetStr() * s.value1[z]), targets[z].GetDEF()));
        }
    }

    //This is a normal hit with a chance to go off.
    public static void NormalHitChance(Unit attacker, Unit[] targets, Skill s, int i)
    {
        //Roll odds.
        if (Random.Range(0, 100) <= s.value2[i]) NormalHit(attacker, targets, s, i);
    }
    #endregion
}
