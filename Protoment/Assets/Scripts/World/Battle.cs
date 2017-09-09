using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Battle : MonoBehaviour
{
    //This class controls the flow of battle. Whos turn it is. How ATB ticks. When ATB ticks, and when battle is resolved.

    //A reference to our two arenas.
    public Arena PlayerArena;
    public Arena EnemyArena;

    //This controls battle speed
    public float speed = 1f;

    //On game updates
    public void Update()
    {
        //Count down the skill handler being busy.
        SkillHandler.busy -= Time.deltaTime * speed;

        //If there's at least one character alive on both sides, and there isn't someone currently taking a turn.
        if (PlayerArena.myParty.GetAllLiving().Count > 0 && EnemyArena.myParty.GetAllLiving().Count > 0 && PlayerArena.myParty.GetAllReady().Count == 0 && EnemyArena.myParty.GetAllReady().Count == 0)
        {
            //Tick the game forward.
            Tick();
        }
        else
        {
            //Take any turns that need taking.
            TakeTurns(PlayerArena, EnemyArena);
            TakeTurns(EnemyArena, PlayerArena);
        }
    }

    //Tick the game update the battle state.
    public void Tick()
    {
        //Tick the battle forward.
        PlayerArena.Tick();
        EnemyArena.Tick();
    }

    //Give each ready unit a turn.
    public void TakeTurns(Arena ally, Arena enemy)
    {
        //Get a list of units from this team ready to take a turn.
        List<Unit> ready = ally.myParty.GetAllReady();

        //For each unit that's ready.
        foreach (Unit u in ready)
        {
            //If no one is currently taking a turn.
            if (SkillHandler.busy <= 0)
            {
                //Set off turn start stuff.
                u.TurnStart();
                //Use a skill.
                SkillHandler.UseSkill(u, u.mySkills[0], ally.myParty, enemy.myParty);
                //Set off end of turn stuff.
                u.TurnEnd();
            }
        }
    }

    //This goes off on Victory.
    public void Victory()
    {
        Debug.Log("You win the battle! Good job. I bet you're stuck at this screen with nothing to look at now, huh?");
    }

    //This goes off on Defeat.
    public void Defeat()
    {
        Debug.Log("You lost. Sucks to be you.");
    }
}
