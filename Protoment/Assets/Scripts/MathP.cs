using UnityEngine;
using System.Collections;

public static class MathP
{
    //This is a static class that functions as a singular place for all common game math to go. Things like the damage formula that will be used all over the project are deposited here so it's easy to mess with them in one singular location.

    //Calculate normal damage.
    public static long GetDamage(long atk, long def)
    {
        long r = (long)((atk * (decimal)Random.Range(.75f, 1.25f)) - def);
        if (r < 0) r = 0;
        return r;
    }
}
