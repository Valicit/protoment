using UnityEngine;
using System.Collections;

public static class MathP
{
    //This is a static class that functions as a singular place for all common game math to go. Things like the damage formula that will be used all over the project are deposited here so it's easy to mess with them in one singular location.

    //Not quite math, but these numbers are used in common formulas.
    public static Vector2[] StatGain = new Vector2[]
    {
        //These are each rank in order.
        new Vector2(0.05f, 0.15f),
        new Vector2(0.15f, 0.25f),
        new Vector2(0.25f, 0.35f),
        new Vector2(0.35f, 0.45f),
        new Vector2(0.45f, 0.55f),
        new Vector2(0.55f, 0.65f),
        new Vector2(0.65f, 0.75f),
        new Vector2(0.75f, 0.85f),
        new Vector2(0.85f, 0.95f),
        new Vector2(0.95f, 1.05f)
    };

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
        decimal r = (atk * (decimal)Random.Range(1.08f, 1.32f));
        r -= (def * (decimal)Random.Range(0.45f, 0.6f));
        //r *= (1000 / (1000 + (def * 3)));
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

    //returns true if x has an advantage over y.
    public static int IsElementAdavantage(Element x, Element y)
    {
        switch (x)
        {
            case Element.Fire:
                if (y == Element.Wood) return 1;
                if (y == Element.Water) return -1;
                break;

            case Element.Water:
                if (y == Element.Fire) return 1;
                if (y == Element.Electric) return -1;
                break;

            case Element.Electric:
                if (y == Element.Water) return 1;
                if (y == Element.Wood) return -1;
                break;

            case Element.Wood:
                if (y == Element.Electric) return 1;
                if (y == Element.Fire) return -1;
                break;

            case Element.Light:
                if (y == Element.Dark) return 1;
                break;

            case Element.Dark:
                if (y == Element.Light) return 1;
                break;
        }
        //Default to no.
        return 0;
    }
}
