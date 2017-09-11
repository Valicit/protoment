using UnityEngine;
using System.Collections;

public static class MathP
{
    //This is a static class that functions as a singular place for all common game math to go. Things like the damage formula that will be used all over the project are deposited here so it's easy to mess with them in one singular location.

    //Calculate normal damage.
    public static long GetDamage(decimal atk, decimal def)
    {
        decimal r = (atk * (decimal)Random.Range(.75f, 1.25f));
        r *= (1000 / (1000 + (def * 3)));
        if (r < 0) r = 0;
        return (long)r;
    }

    //Roll for crit, and return crit mod.
    public static float GetCritDamage(Unit attacker, float offset)
    {
        //Return true if we got a crit.
        if (Random.Range(0, 100) < attacker.Crit + offset) return attacker.CritDMG;

        //Return normal if it's not a crit.
        return 1f;
    }
    public static float GetCritDamage(Unit attacker)
    {
        //Return the result with no offset.
        return GetCritDamage(attacker, 0);
    }
}
