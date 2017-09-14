using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit
{
    //This class is the base class for every unit in the game. If it can be placed in combat at all, it's got one of these. Even if it can't be placed in combat, it might have one of these.

    //This is unit information.
    public int level = 1;
    public long exp = 0;
    public string uName = "Name";
    public string job = "Job";
    public Element uElement = Element.Fire;
    public Rarity uRarity = Rarity.Common;
    public Skill[] mySkills = new Skill[3];
    public Sprite uSprite;

    //These are all the units current stats.
    public long mHP = 100;
    public long STR = 10;
    public long DEF = 10;
    public long INT = 10;
    public long SPR = 10;
    public long DEX = 10;
    public long AGI = 10;
    public float Crit = 0.05f;
    public float CritDMG = 1.5f;
    public float Speed = 1;

    //These are the units bonus stats.
    public long bHP = 100;
    public long bSTR = 10;
    public long bDEF = 10;
    public long bINT = 10;
    public long bSPR = 10;
    public long bDEX = 10;
    public long bAGI = 10;

    //These are the units stat modifiers.
    public decimal modHP = 1m;
    public decimal modSTR = 1m;
    public decimal modDEF = 1m;
    public decimal modINT = 1m;
    public decimal modSPR = 1m;
    public decimal modDEX = 1m;
    public decimal modAGI = 1m;

    //Current Battle Stuff
    public long cHP = 100;
    public float atb = 0;
    public List<string> textQueue = new List<string>();
    public List<Color> textColor = new List<Color>();
    public List<StatusEffect> myStatusEffects = new List<StatusEffect>();

    //This ticks the character forward in battle, updating ATB and anything else.
    public void Tick()
    {
        //Move the atb forward.
        atb += (GetSpeed() / 100) * Battle.speed;
        if (atb > 100) atb = 100;

        //Work through passive skills.
        for (int i = 0; i < mySkills.Length; i++)
        {
            mySkills[i].Tick(this);
        }
    }

    //This happens when the turn starts.
    public void TurnStart()
    {
        atb = 0;

        //Tick down and remove status effects.
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            if (myStatusEffects[i].percentDamage > 0) TakeHit((long)((float)GetmHP() * myStatusEffects[i].percentDamage), false, 1f);
            if (myStatusEffects[i].percentHealing > 0) TakeHit((long)((float)GetmHP() * myStatusEffects[i].percentHealing), true, 1f);
            if (!myStatusEffects[i].permanent) myStatusEffects[i].duration--;
        }
        myStatusEffects.RemoveAll(n => n.duration <= 0);
    }

    //This happens when the turn ends.
    public void TurnEnd()
    {
        //Count down our cooldowns.
        for (int i = 0; i < mySkills.Length; i++)
        {
            mySkills[i].CountCD();
        }
    }

    //This checks if the character is ready to take a turn.
    public bool TurnReady()
    {
        if (atb >= 100) return true;
        else return false;
    }

    //This checks if the unit is alive.
    public bool IsAlive()
    {
        if (cHP > 0) return true;
        else return false;
    }

    //This makes the unit get hit by something.
    public void TakeHit(long d, bool isHealing, float critMod)
    {
        //Add crit.
        d = (long)(d * critMod);

        //Factor damage taken mod.
        if(!isHealing) d = (long)(d * GetDamageReduction());

        //Add damage text.
        string dString = d.ToString();
        if (critMod > 1) dString += "!!!";
        textQueue.Add(dString);

        //If this is healing, cause healing.
        if (isHealing)
        {
            TakeHealing(d);
            textColor.Add(Color.green);
        }
        else //Otherwise do damage.
        {
            TakeDamage(d);
            textColor.Add(Color.white);
        }
    }

    //Take some damage!
    public void TakeDamage(long d) //Hah. Long d. Remember kids, naming conventions can be hilarious.
    {
        cHP -= d;
        if (cHP < 0) cHP = 0;
    }

    //Take some healing.
    public void TakeHealing(long d)
    {
        cHP += d;
        if (cHP > GetmHP()) cHP = GetmHP();
    }

    //Add a status effect to the character.
    public void AddStatusEffect(StatusEffect s)
    {
        //If it stacks, just add it to the list.
        if (s.isStackable)
        {
            myStatusEffects.Add(s);
        }
        else
        {
            //Make sure it's not already there.
            StatusEffect r = myStatusEffects.Find(n => n.statusName == s.statusName);
            if (r != null) r.duration = Mathf.Max(r.duration, s.duration);
            else myStatusEffects.Add(s);
        }
    }

    //Set off any counter attacks.
    public void Counter(AttackData data)
    {
        //For each status effect we have.
        foreach (StatusEffect se in myStatusEffects)
        {
            if (se.counterStatus != null)
            {
                //Roll.
                StatusEffect counterStatus = StatusEffect.Instantiate(se.counterStatus);
                counterStatus.duration = se.counterStatusDuration;
                if (Random.Range(0, 100) < se.counterStatusChance) data.actor.AddStatusEffect(counterStatus);
            }
        }
    }

    #region GetStats
    public long GetmHP() { return (long)(mHP * modHP); }
    public long GetSTR() { return (long)(STR * modSTR * GetSTRstatus()); }
    public long GetDEF() { return (long)(DEF * modDEF * GetDEFstatus()); }
    public long GetINT() { return (long)(INT * modINT); }
    public long GetSPR() { return (long)(SPR * modSPR); }
    public long GetDEX() { return (long)(DEX * modDEX); }
    public long GetAGI() { return (long)(AGI * modAGI); }
    public float GetCrit() { return Crit; }
    public float GetCritDMG() { return CritDMG; }
    public float GetSpeed() { return Speed * GetSpeedStatus(); }
    public float GetDamageReduction()
    {
        float r = 1f;
        //Factor status effects.
        foreach (StatusEffect se in myStatusEffects)
        {
            r += se.takenDamageMod;
        }
        return r;
    }
    public decimal GetSTRstatus() //TODO: Add in other stat mods from Status Effects.
    {
        float r = 1;
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            r *= myStatusEffects[i].STRmod;
        }
        return (decimal)r;
    }
    public decimal GetDEFstatus() //TODO: Add in other stat mods from Status Effects.
    {
        float r = 1;
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            r *= myStatusEffects[i].DEFmod;
        }
        return (decimal)r;
    }
    public float GetSpeedStatus()
    {
        float r = 1;
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            r *= myStatusEffects[i].speedMod;
        }
        return r;
    }
    public long GetStat(StatBase s)
    {
        switch (s)
        {
            case StatBase.None:
                return 0;
            case StatBase.HP:
                return GetmHP();
            case StatBase.STR:
                return GetSTR();
            case StatBase.DEF:
                return GetDEF();
            case StatBase.INT:
                return GetINT();
            case StatBase.SPR:
                return GetSPR();
            case StatBase.DEX:
                return GetDEX();
            case StatBase.AGI:
                return GetAGI();
        }

        //Default to STR.
        Debug.Log("Somehow got a stat that wasn't really there?");
        return GetSTR();
    }
    #endregion

    //Return a new unit of the specified class.
    public static Unit NewUnit(UnitData u)
    {
        //Create a new unit.
        Unit r = new Unit();

        //Set basic info.
        if (u == null) Debug.Log("Not loading Properly");
        r.uName = u.uName;
        r.job = u.job;
        r.mySkills[0] = Skill.Instantiate(u.skill1);
        r.mySkills[1] = Skill.Instantiate(u.skill2);
        r.mySkills[2] = Skill.Instantiate(u.skill3);
        r.uRarity = u.uRarity;
        r.uSprite = u.unitSprite;

        //Set all stats properly.
        r.mHP = u.HP;
        r.STR = u.STR;
        r.DEF = u.DEF;
        r.INT = u.INT;
        r.SPR = u.SPR;
        r.DEX = u.DEX;
        r.AGI = u.AGI;
        r.Crit = u.crit;
        r.CritDMG = u.critDMG;
        r.Speed = u.speed;

        //Set default values.
        r.cHP = r.GetmHP();

        //TEMP
        r.atb = Random.Range(0, 100);

        //Return the result.
        return r;
    }
}
