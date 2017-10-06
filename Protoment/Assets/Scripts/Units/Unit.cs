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
    public int rank = 1;
    public float classExpMod;
    public List<Skill> mySkills = new List<Skill>();
    public Sprite uSprite;
    public UnitData myData;

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
    public float modHP = 1;
    public float modSTR = 1;
    public float modDEF = 1;
    public float modINT = 1;
    public float modSPR = 1;
    public float modDEX = 1;
    public float modAGI = 1;

    //This is the units equipment.
    public Equipment weapon;
    public Equipment armor;
    public Equipment accessory;

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
        atb += (GetSpeed() / 100) * Battle.speed * Time.deltaTime;
        if (atb > 1000) atb = 1000;

        //Work through passive skills.
        for (int i = 0; i < mySkills.Count; i++)
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
        }

        //Heal for any regen effects.
        if (GetRegen() > 0) TakeHit(GetRegen(), true, 1f);
    }

    //This happens when the turn ends.
    public void TurnEnd()
    {
        //Count down our cooldowns.
        for (int i = 0; i < mySkills.Count; i++)
        {
            mySkills[i].CountCD();
        }

        //Trigger end turn skills.
        foreach (StatusEffect se in myStatusEffects.FindAll(n => n.triggerTurnEnd))
        {
            //If this status has a triggered skill.
            if (se.triggerSkill != null && se.triggerTurnEnd)
            {
                TriggerSkill(se.triggerSkill);
            }
        }

        //Tick down and remove status effects.
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            if (!myStatusEffects[i].permanent) myStatusEffects[i].duration--;
        }
        myStatusEffects.RemoveAll(n => n.duration <= 0 && !n.permanent);
        myStatusEffects.RemoveAll(n => n.applier == null && n.permanent); // Remove passive statuses.
    }

    //Trigger a skill on not our turn.
    public void TriggerSkill(Skill s)
    {
        Battle.triggerUnit = this;
        Battle.battle.UseSkill(s);
        Battle.triggerUnit = null;
    }

    //This checks if the character is ready to take a turn.
    public bool TurnReady()
    {
        if (atb >= 1000) return true;
        else return false;
    }

    //Do this when we die.
    public void OnDeath()
    {
        //Set off any death triggers.
        foreach (StatusEffect se in myStatusEffects.FindAll(n => n.triggerOnDeath))
        {
            TriggerSkill(se.triggerSkill);
        }
    }

    //Do this when we get hit by a critical attack.
    public void OnGetCrit()
    {
        //Set off any crit triggers.
        foreach (StatusEffect se in myStatusEffects.FindAll(n => n.triggerOnGetCrit))
        {
            TriggerSkill(se.triggerSkill);
        }
    }

    //This checks if the unit is alive.
    public bool IsAlive()
    {
        if (cHP > 0) return true;
        else return false;
    }

    //This makes the unit get hit by something.
    public void TakeHit(long d, bool isHealing, float critMod, int elementAdvantage)
    {
        //Add crit.
        d = (long)(d * critMod);

        //Factor damage taken mods.
        if (!isHealing)
        {
            d = (long)(d * GetDamageReduction());
            if (myStatusEffects.Find(n => n.invulnerable)) d = 0;

            //Remove status effects that get removed on damage.
            myStatusEffects.RemoveAll(n => n.removeOnDamage);
        }

        //Add damage text.
        string dString = d.ToString();
        if (critMod > 1)
        {
            dString += "!!!";
            OnGetCrit();
        }
        textQueue.Add(dString);

        //If this is healing, cause healing.
        if (isHealing)
        {
            TakeHealing(d);
            textColor.Add(Color.green);
        }
        else //Otherwise do damage.
        {
            if (elementAdvantage == 1) textColor.Add(Color.magenta);
            else if (elementAdvantage == -1) textColor.Add(Color.blue);
            else textColor.Add(Color.white);
            TakeDamage(d);
        }
    }
    public void TakeHit(long d, bool isHealing, float critMod) { TakeHit(d, isHealing, critMod, 0); }

    //Take some damage!
    public void TakeDamage(long d) //Hah. Long d. Remember kids, naming conventions can be hilarious.
    {
        //Remove from any shields present first.
        foreach (StatusEffect se in myStatusEffects.FindAll(n => n.isShield))
        {
            long absorbs = (long)Mathf.CeilToInt(Mathf.Min(se.variableValue, d));
            se.variableValue -= absorbs;
            d = (long)Mathf.Max(0, d - absorbs);
            Debug.Log("Absobed " + absorbs + " Left:" + se.variableValue);
        }
        myStatusEffects.RemoveAll(n => n.isShield && n.variableValue <= 0);

        cHP -= d;
        if (cHP <= 0)
        {
            cHP = 0;
            OnDeath();
        }
    }

    //Take some healing.
    public void TakeHealing(long d)
    {
        cHP += d;
        if (cHP > GetmHP()) cHP = GetmHP();
    }

    //Dodge an attack.
    public void TakeMiss()
    {
        //Add damage text.
        string dString = "Miss!";
        textQueue.Add(dString);
        textColor.Add(Color.white);
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
            if (r != null)
            {
                r.duration = Mathf.Max(r.duration, s.duration);
                if(s.isShield && r.isShield) r.variableValue += s.variableValue;
            }
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
                //For each counter.
                for (int i = 0; i < se.counterStatus.Length; i++)
                {
                    //Roll.
                    StatusEffect counterStatus = StatusEffect.Instantiate(se.counterStatus[i]);
                    counterStatus.duration = se.counterStatusDuration[i];
                    if (Random.Range(0, 100) < se.counterStatusChance[i] && data.actor != this) data.actor.AddStatusEffect(counterStatus);
                }
            }
        }
    }

    //Add exp.
    public void AddExp(long n)
    {
        exp += (long)(n * GetEquipStatsPercentage(EquipStats.EXP));

        //If we got a level.
        while (exp >= GetENext(level) && level < MathP.maxLevels[rank - 1])
        {
            //Subtract the exp and level up.
            exp -= GetENext(level);
            LevelUp();
        }
    }

    //Level up the unit.
    public void LevelUp()
    {
        mHP += (long)Mathf.Max(1, (myData.HP * Random.Range(MathP.StatGain[rank - 1].x * 4, MathP.StatGain[rank - 1].y * 4)));
        STR += (long)Mathf.Max(1, (myData.STR * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        DEF += (long)Mathf.Max(1, (myData.DEF * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        INT += (long)Mathf.Max(1, (myData.INT * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        SPR += (long)Mathf.Max(1, (myData.SPR * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        DEX += (long)Mathf.Max(1, (myData.DEX * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        AGI += (long)Mathf.Max(1, (myData.AGI * Random.Range(MathP.StatGain[rank - 1].x, MathP.StatGain[rank - 1].y)));
        cHP = GetmHP();
        level++;
    }
    public void LevelUp(int times)
    {
        for (int i = 0; i < times; i++)
        {
            LevelUp();
        }
    }

    //Reap the unit for stuff.
    public void Reap()
    {
        //Add mana.
        Player.mana += MathP.GetReapValue(rank);
    }

    #region GetStats
    public long GetmHP() { return (long)((mHP + GetEquipStats(EquipStats.FlatHP)) * modHP * GetStatStatus(StatBase.HP) * GetEquipStatsPercentage(EquipStats.HP)); }
    public long GetSTR() { return (long)((STR + GetEquipStats(EquipStats.FlatSTR)) * modSTR * GetStatStatus(StatBase.STR) * GetEquipStatsPercentage(EquipStats.STR)); }
    public long GetDEF() { return (long)((DEF + GetEquipStats(EquipStats.FlatDEF)) * modSTR * GetStatStatus(StatBase.DEF) * GetEquipStatsPercentage(EquipStats.DEF)); }
    public long GetINT() { return (long)((INT + GetEquipStats(EquipStats.FlatINT)) * modSTR * GetStatStatus(StatBase.INT) * GetEquipStatsPercentage(EquipStats.INT)); }
    public long GetSPR() { return (long)((SPR + GetEquipStats(EquipStats.FlatSPR)) * modSTR * GetStatStatus(StatBase.SPR) * GetEquipStatsPercentage(EquipStats.SPR)); }
    public long GetDEX() { return (long)((DEX + GetEquipStats(EquipStats.FlatDEX)) * modSTR * GetStatStatus(StatBase.DEX) * GetEquipStatsPercentage(EquipStats.DEX)); }
    public long GetAGI() { return (long)((AGI + GetEquipStats(EquipStats.FlatAGI)) * modSTR * GetStatStatus(StatBase.AGI) * GetEquipStatsPercentage(EquipStats.AGI)); }
    public float GetCrit() { return Crit + GetEquipStats(EquipStats.Crit); }
    public float GetCritDMG() { return CritDMG + (GetEquipStatsPercentage(EquipStats.CritDMG) - 1); }
    public float GetSpeed() { return ((Speed + GetEquipStats(EquipStats.Speed)) * GetStatStatus(StatBase.Speed)) + ((float)level * 0.1f); }
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
    public float GetStatStatus(StatBase s)
    {
        float r = 1;
        for (int i = 0; i < myStatusEffects.Count; i++)
        {
            switch (s)
            {
                case StatBase.HP:
                    r *= myStatusEffects[i].HPmod;
                    break;
                case StatBase.STR:
                    r *= myStatusEffects[i].STRmod;
                    break;
                case StatBase.DEF:
                    r *= myStatusEffects[i].DEFmod;
                    break;
                case StatBase.INT:
                    r *= myStatusEffects[i].INTmod;
                    break;
                case StatBase.SPR:
                    r *= myStatusEffects[i].SPRmod;
                    break;
                case StatBase.DEX:
                    r *= myStatusEffects[i].DEXmod;
                    break;
                case StatBase.AGI:
                    r *= myStatusEffects[i].AGImod;
                    break;
                case StatBase.Speed:
                    r *= myStatusEffects[i].speedMod;
                    break;
            }
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
            case StatBase.Speed:
                return (long)GetSpeed();
            case StatBase.Flat:
                return 1;
            case StatBase.CurrentHP:
                return cHP;
            case StatBase.MissingHP:
                return GetmHP() - cHP;
        }

        //Default to STR.
        Debug.Log("Somehow got a stat that wasn't really there?");
        return GetSTR();
    }

    //get exp to next level.
    public long GetENext(float level)
    {
        float r = (0.04f * Mathf.Pow(level,3)) + (0.8f * Mathf.Pow(level, 2)) + (2 * level);
        r *= classExpMod;
        return (long)r;
    }

    //Get the currently equipped item in a given slot.
    public Equipment GetEquipped(EquipType et)
    {
        switch (et)
        {
            case EquipType.Weapon:
                return weapon;
            case EquipType.Armor:
                return armor;
            case EquipType.Accessory:
                return accessory;
        }
        Debug.Log("Something has gone terribly wrong with finding an equipped item.");
        return null;
    }

    //Get all values of one stat on all of our equipment.
    public float GetEquipStats(EquipStats es)
    {
        float r = 0;
        if (weapon != null) r += weapon.GetStatValue(es);
        if (armor != null) r += armor.GetStatValue(es);
        if (accessory != null) r += accessory.GetStatValue(es);
        return r;
    }
    public float GetEquipStatsPercentage(EquipStats es)
    {
        float r = 0;
        if (weapon != null) r += weapon.GetStatValue(es);
        if (armor != null) r += armor.GetStatValue(es);
        if (accessory != null) r += accessory.GetStatValue(es);
        return 1 + (r / 100);
    }

    //Get my regen from equips and stuff.
    public long GetRegen()
    {
        //Hold our result.
        long r = 0;

        //Add equipment, flat and percentage based.
        r += Mathf.CeilToInt(GetEquipStats(EquipStats.FlatRegen));
        r += Mathf.CeilToInt(GetmHP() * (GetEquipStatsPercentage(EquipStats.Regen) - 1));

        //Return the result.
        return r;
    }

    //Get my thorns from equips and stuff.
    public long GetThorns(long d)
    {
        //Hold our result.
        long r = 0;

        //Add equipment, flat and percentage based.
        r += Mathf.CeilToInt(GetEquipStats(EquipStats.FlatThorns));
        r += Mathf.CeilToInt(d * (GetEquipStatsPercentage(EquipStats.Thorns) - 1));

        //Check status effects.
        foreach (StatusEffect se in myStatusEffects.FindAll(n => n.isThorns))
        {
            r += Mathf.CeilToInt(d * se.variableValue);
        }

        //Return the result.
        return r;
    }
    #endregion

    //Return a new unit of the specified class.
    public static Unit NewUnit(UnitData u, int level)
    {
        //Create a new unit.
        Unit r = new Unit();

        //Set basic info.
        if (u == null) Debug.Log("Not loading Properly");
        r.uName = u.uName;
        r.job = u.job;
        r.uElement = u.uElement;
        foreach (Skill s in u.skills)
        {
            r.mySkills.Add(Skill.Instantiate(s));
        }
        r.uRarity = u.uRarity;
        r.classExpMod = u.classExpMod;
        r.rank = (int)u.uRarity + 1;
        r.uSprite = u.unitSprite;
        r.myData = UnitData.Instantiate(u);

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
        r.LevelUp(level - 1);

        //Return the result.
        return r;
    }

    //Roll a unit.
    public static Unit NewUnit(UnitData u)
    {
        return NewUnit(u, 1);
    }
}
