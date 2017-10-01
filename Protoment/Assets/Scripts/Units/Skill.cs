using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This helps decide what is and is not a valid target.
public enum ValidTarget
{
    SingleEnemy = 0,
    SingleFrontEnemy = 1,
    SingleAlly = 101,
}

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    //This class has all of the methods a skill needs to work.

    //These are a list of variables commonly needed by skills.
    public string displayName = "Skill";
    public ValidTarget validTarget = ValidTarget.SingleEnemy;
    public int cd = 0;
    public int maxCD = 0;
    public bool isCounterable = true;
    public bool isPassive = false;
    public StatusEffect passiveStatus;
    public StatusEffect passivePartyStatus;
    public StatusEffect passiveEnemyStatus;
    public SkillComponent[] components;

    //Skill use requires attack data, populated by all the data that can be collected before the skill starts.
    public void UseSkill(AttackData data)
    {
        //Announce the skill.
        data.actor.textQueue.Add(displayName);
        data.actor.textColor.Add(Color.yellow);

        //For each component.
        foreach (SkillComponent component in components)
        {
            component.UseComponent(data);
            data.selectedUnit = null;
        }

        //If this is counterable.
        if (isCounterable)
        {
            //Give all affected a chance to counter.
            foreach (Unit u in data.affectedTargets)
            {
                u.Counter(data);
            }
        }

        //Put the skill on cooldown.
        cd = maxCD;
    }

    //This applies the status effect to everyone in the scene.
    public void Tick(Unit me)
    {
        //Get relevant data.
        Party ally;
        Party enemy;
        if (Battle.battle.PlayerArena.myParty.GetAllUnits().Contains(me))
        {
            ally = Battle.battle.PlayerArena.myParty;
            enemy = Battle.battle.EnemyArena.myParty;
        }
        else
        {
            enemy = Battle.battle.PlayerArena.myParty;
            ally = Battle.battle.EnemyArena.myParty;
        }

        //If I should have a passive effect.
        if (passiveStatus != null)
        {
            me.AddStatusEffect(passiveStatus);
        }

        //If the party should have an effect.
        if (passivePartyStatus != null)
        {
            foreach (Unit u in ally.GetAllLiving())
            {
                u.AddStatusEffect(passivePartyStatus);
            }
        }

        //If the enemy party should have an effect.
        if (passiveEnemyStatus != null)
        {
            foreach (Unit u in enemy.GetAllLiving())
            {
                u.AddStatusEffect(passiveEnemyStatus);
            }
        }
    }

    //This checks if the skill is ready to use.
    public bool IsReady()
    {
        if (cd == 0 && !isPassive) return true;
        else return false;
    }

    //Count down the cooldown by one.
    public void CountCD()
    {
        cd--;
        if (cd < 0) cd = 0;
    }

    //Get whether the attack is being made against a valid target.
    public bool IsValidTarget(Unit target, AttackData data)
    {
        switch (validTarget)
        {
            case ValidTarget.SingleEnemy:
                if (data.defendingParty.ValidateSingleRandom().Contains(target)) return true;
                break;
            case ValidTarget.SingleFrontEnemy:
                if(data.defendingParty.ValidateSingleFrontLine().Contains(target)) return true;
                break;
            case ValidTarget.SingleAlly:
                if (data.actorParty.ValidateSingleRandom().Contains(target)) return true;
                break;
        }

        //Default to false.
        return false;
    }
}
