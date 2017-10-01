using UnityEngine;
using System.Collections;

public static class MathP
{
    //This is a static class that functions as a singular place for all common game math to go. Things like the damage formula that will be used all over the project are deposited here so it's easy to mess with them in one singular location.

    //Not quite math, but these numbers are used in common formulas.
    public static float minStatGain = 0.45f;
    public static float maxStatGain = 0.55f;

    //These are max levels at different ranks.
    public static int[] maxLevels = new int[] {
        20,
        40,
        60,
        80,
        100,
        200,
        500,
        1000,
        5000,
        9999
    };

    //Get rank value.
    public static long GetReapValue(int rank)
    {
        long r = 1;
        for (int i = 0; i < rank; i++)
        {
            r *= i +1;
        }
        return r;
    }

    //Calculate normal damage.
    public static long GetDamage(decimal atk, decimal def)
    {
        decimal r = (atk * (decimal)Random.Range(0.9f, 1.1f));
        r -= (def * (decimal)Random.Range(0.15f, 0.2f));
        r *= (1000 / (1000 + (def * 3)));
        //r -= (def * (decimal)Random.Range(0.15f, 0.2f));
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
