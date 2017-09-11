using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct AttackData
{
    //This class contains all of the relevant data about one attack that is made. It is passed along everywhere it needs to go, before being added to a list of attacks made in this battle.

    //These are references to all affected units.
    public Unit actor;
    public Party actorParty;
    public Party defendingParty;
    public List<Unit> affectedTargets;
    public Skill skillUsed;

    //This is a reference to attack data that may be valuable at some point.
    public long damageDone;
    public long healingDone;
    public int effectsInflicted;
    public int effectsCleansed;
}
