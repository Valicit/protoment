using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This helps decide what is and is not a valid target.
public enum ValidTarget
{
    AnyEnemy = 0,
    AnyFrontEnemy = 1,
    AnyAlly = 101,
}

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    //This class has all of the methods a skill needs to work.

    //These are a list of variables commonly needed by skills.
    public string displayName = "Skill";
    public ValidTarget validTarget = ValidTarget.AnyEnemy;
    private float wait = 1f;
    public int cd = 0;
    public int maxCD = 0;
    public bool isCounterable = true;
    public bool isPassive = false;
    public bool isTriggered = false;
    public StatusEffect passiveStatus;
    public StatusEffect passivePartyStatus;
    public StatusEffect passiveEnemyStatus;
    public Skill statusTriggerSkill;
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

        //Make the battle wait.
        Battle.battle.wait += wait;
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
            me.AddStatusEffect(GetPassiveStatus(passiveStatus, me));
        }

        //If the party should have an effect.
        if (passivePartyStatus != null)
        {
            foreach (Unit u in ally.GetAllLiving())
            {
                u.AddStatusEffect(GetPassiveStatus(passivePartyStatus, me));
            }
        }

        //If the enemy party should have an effect.
        if (passiveEnemyStatus != null)
        {
            foreach (Unit u in enemy.GetAllLiving())
            {
                u.AddStatusEffect(GetPassiveStatus(passiveEnemyStatus, me));
            }
        }
    }

    //Get a passive status.
    public StatusEffect GetPassiveStatus(StatusEffect b, Unit me)
    {
        StatusEffect se = StatusEffect.Instantiate(b);
        if (statusTriggerSkill != null) se.triggerSkill = statusTriggerSkill;
        se.applier = me;
        return se;
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
            case ValidTarget.AnyEnemy:
                if (data.defendingParty.ValidateSingleRandom().Contains(target)) return true;
                break;
            case ValidTarget.AnyFrontEnemy:
                if(data.defendingParty.GetFrontLine().Contains(target)) return true;
                break;
            case ValidTarget.AnyAlly:
                if (data.actorParty.ValidateSingleRandom().Contains(target)) return true;
                break;
        }

        //Default to false.
        return false;
    }
}
