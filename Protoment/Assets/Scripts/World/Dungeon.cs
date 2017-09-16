using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class Dungeon : ScriptableObject
{
    //This controls one dungeon, and has all the variables needed to make it interesting.

    //This is an array of unit arrays that can be loaded into a party.
    public int currentWave = 0;
    public Wave[] waves;

    //Get the current wave.
    public Party GetParty(int wave)
    {
        //Make a new party.
        Party p = new Party();

        //Insert Units. There was no reason not to use a nested loop here. But I am tired and I realized half way through making it. Go me.
        if (waves[wave].unit00 != null) p.myUnits[0, 0] = Unit.NewUnit(waves[wave].unit00, waves[wave].Level[0]);
        if (waves[wave].unit01 != null) p.myUnits[0, 1] = Unit.NewUnit(waves[wave].unit01, waves[wave].Level[1]);
        if (waves[wave].unit02 != null) p.myUnits[0, 2] = Unit.NewUnit(waves[wave].unit02, waves[wave].Level[2]);
        if (waves[wave].unit10 != null) p.myUnits[1, 0] = Unit.NewUnit(waves[wave].unit10, waves[wave].Level[3]);
        if (waves[wave].unit11 != null) p.myUnits[1, 1] = Unit.NewUnit(waves[wave].unit11, waves[wave].Level[4]);
        if (waves[wave].unit12 != null) p.myUnits[1, 2] = Unit.NewUnit(waves[wave].unit12, waves[wave].Level[5]);
        if (waves[wave].unit20 != null) p.myUnits[2, 0] = Unit.NewUnit(waves[wave].unit20, waves[wave].Level[6]);
        if (waves[wave].unit21 != null) p.myUnits[2, 1] = Unit.NewUnit(waves[wave].unit21, waves[wave].Level[7]);
        if (waves[wave].unit22 != null) p.myUnits[2, 2] = Unit.NewUnit(waves[wave].unit22, waves[wave].Level[8]);

        //Return the party.
        return p;
    }
}
