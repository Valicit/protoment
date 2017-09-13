using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class SkillDB : ScriptableObject
{
    public static Dictionary<string, Skill> db;

    //When initialized.
    public void OnEnable()
    {
        //Add everything.
        db = new Dictionary<string, Skill>
        {
            //Novice skills.
            { "Lucky Strike",  new LuckyStrike()}
        };
    }
}