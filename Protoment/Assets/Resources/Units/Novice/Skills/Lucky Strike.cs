using UnityEngine;
using System.Collections;

public class LuckyStrike : Skill
{
    //Variable's N'Stuff.
    public float damageMod = 2;
    public float thirdHitChance = 50;

    //Init.
    public LuckyStrike()
    {
        displayName = "Lucky Strike";
        validTarget = ValidTarget.ClosestEnemy;
    }

    //Use the skill!
    public override void UseSkill(AttackData data)
    {
        //Get a target.
        Unit t = data.actor;
        if (data.selectedUnit != null) t = data.selectedUnit;
        else t = data.defendingParty.GetSingleFrontLine();
        if (t == null) Debug.Log("It sure does!?");
        //Hit it.
        t.TakeHit(MathP.GetDamage((decimal)(data.actor.GetSTR() * damageMod), t.GetDEF()), false, 1.0f, data);
        t.TakeHit(MathP.GetDamage((decimal)(data.actor.GetSTR() * damageMod), t.GetDEF()), false, 1.0f, data);
        if(Random.Range(0, 100) < thirdHitChance) t.TakeHit(MathP.GetDamage((decimal)(data.actor.GetSTR() * damageMod), t.GetDEF()), false, 1.0f, data);
    }
}
