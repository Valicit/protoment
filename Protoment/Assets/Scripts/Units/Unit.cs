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
    public int[] cooldowns = new int[3];
    public List<string> textQueue = new List<string>();
    public List<Color> textColor = new List<Color>();
    public List<StatusEffect> myStatusEffects = new List<StatusEffect>();

    //This ticks the character forward in battle, updating ATB and anything else.
    public void Tick()
    {
        //Move the atb forward.
        atb += Speed;
        if (atb > 100) atb = 100;
        if (TurnReady()) Debug.Log("Unit is READY!");
    }

    //This happens when the turn starts.
    public void TurnStart()
    {
        atb = 0;
    }

    //This happens when the turn ends.
    public void TurnEnd()
    {

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

    //Take some damage!
    public void TakeDamage(long d) //Hah. Long d. Remember kids, naming conventions can be hilarious.
    {
        cHP -= d;
        if (cHP < 0) cHP = 0;
        textQueue.Add(d.ToString());
        textColor.Add(Color.white);
    }

    #region GetStats
    public long GetmHP()
    {
        return (long)(mHP * modHP);
    }

    public long GetStr()
    {
        return (long)(STR * modSTR);
    }

    public long GetDEF()
    {
        return (long)(DEF * modDEF);
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
        r.mySkills[0] = u.skill1;
        r.mySkills[1] = u.skill2;
        r.mySkills[2] = u.skill3;
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
