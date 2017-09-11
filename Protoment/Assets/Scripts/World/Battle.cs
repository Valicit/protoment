using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Battle : MonoBehaviour
{
    //This class controls the flow of battle. Whos turn it is. How ATB ticks. When ATB ticks, and when battle is resolved.

    //A reference to our two arenas.
    public Arena PlayerArena;
    public Arena EnemyArena;

    //A reference to our UI objects.
    public Button[] SkillButtons;
    public Text[] SkillButtonText;
    public Button AutoButton;

    //This controls battle speed
    public static float speed = 0.1f;

    //This is if the player side is controlling its characters or not.
    public static bool auto = false;
    public static int selectedSkill = 0;
    public static Unit selectedUnit;
    public static Unit ManualUnit;

    //On game updates
    public void Update()
    {
        //Count down the skill handler being busy.
        SkillHandler.busy -= Time.deltaTime * speed;

        //Update the UI.
        UpdateUI();

        //If there's at least one character alive on both sides.
        if (PlayerArena.myParty.GetAllLiving().Count > 0 && EnemyArena.myParty.GetAllLiving().Count > 0)
        {
            //If there are units waiting to take a turn, do turn stuff!
            if (PlayerArena.myParty.GetAllReady().Count > 0 || EnemyArena.myParty.GetAllReady().Count > 0)
            {
                //If it's on auto, go ahead and take turns for both parties as normal.
                if (auto)
                {
                    //Take any turns that need taking.
                    TakeTurnsAuto(PlayerArena, EnemyArena);
                    TakeTurnsAuto(EnemyArena, PlayerArena);
                }
                else
                {
                    //Enemies take turns as normal.
                    TakeTurnsAuto(EnemyArena, PlayerArena);

                    //The first ready ally takes a turn.
                    if (PlayerArena.myParty.GetAllReady().Count > 0 && ManualUnit == null)
                    {
                        ManualUnit = PlayerArena.myParty.GetAllReady()[0];
                    }
                    if (ManualUnit != null && selectedUnit != null) TakeTurnManual(PlayerArena, EnemyArena, ManualUnit);
                }
            }
            else
            {
                //Tick the game forward.
                Tick();
            }
        }
    }

    //Update the UI stuff.
    public void UpdateUI()
    {
        //If we don't have a unit selected, hide all the skill buttons.
        if (ManualUnit == null)
        {
            for (int i = 0; i < SkillButtons.Length; i++)
            {
                SkillButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < SkillButtons.Length; i++)
            {
                //Otherwise, show the proper info and such.
                SkillButtons[i].gameObject.SetActive(true);
                string text = ManualUnit.mySkills[i].displayName;
                if (ManualUnit.mySkills[i].GetCD() > 0)
                {
                    text += " CD: " + ManualUnit.mySkills[i].GetCD();
                    SkillButtons[i].interactable = false;
                }
                else if (ManualUnit.mySkills[i].isPassive) SkillButtons[i].interactable = false;
                else SkillButtons[i].interactable = true;
                SkillButtonText[i].text = text;
            }
        }
    }

    //Tick the game update the battle state.
    public void Tick()
    {
        //Tick the battle forward.
        PlayerArena.Tick();
        EnemyArena.Tick();

        //Use counter attacks.
        foreach (Unit u in PlayerArena.myParty.GetAllLiving())
        {
            if(u.lastAttacker != null) u.UseCounter();
        }
        foreach (Unit u in EnemyArena.myParty.GetAllLiving())
        {
            if (u.lastAttacker != null) u.UseCounter();
        }
    }

    //Give each ready unit a turn.
    public void TakeTurnsAuto(Arena ally, Arena enemy)
    {
        //Get a list of units from this team ready to take a turn.
        List<Unit> ready = ally.myParty.GetAllReady();

        //For each unit that's ready.
        foreach (Unit u in ready)
        {
            //If no one is currently taking a turn.
            if (SkillHandler.busy <= 0)
            {
                //If the character is not stunned.
                if (u.myStatusEffects.Find(n => n.preventAction) == null)
                {
                    //Set off turn start stuff.
                    u.TurnStart();

                    //Choose a skill.
                    Skill s = SkillHandler.ChooseSkill(u, ally.myParty, enemy.myParty);

                    //Use a skill.
                    SkillHandler.UseSkillAuto(u, s, ally.myParty, enemy.myParty);
                    u.tookTurn = true;

                    //Set off end of turn stuff.
                    u.TurnEnd();
                }
                else //If they are stunned, this is a lot more simple.
                {
                    //Start the turn.
                    u.TurnStart();

                    //End the turn. Haha.
                    u.TurnEnd();
                }
            }
        }
    }

    //Give a manual character a turn.
    public void TakeTurnManual(Arena ally, Arena enemy, Unit u)
    {
        //If the character is not stunned.
        if (u.myStatusEffects.Find(n => n.preventAction) == null)
        {
            //Start the turn.
            u.TurnStart();

            //Do a thing.
            SkillHandler.UseSkillManual(ManualUnit, ManualUnit.mySkills[selectedSkill], PlayerArena.myParty, EnemyArena.myParty, selectedUnit);

            //Deselect any units.
            selectedUnit = null;
            selectedSkill = 0;
            ManualUnit = null;

            //End the turn.
            u.TurnEnd();
        }
        else
        {
            u.TurnStart();
            //End the turn. Haha.
            u.TurnEnd();

            //Deselect any units.
            selectedUnit = null;
            selectedSkill = 0;
            ManualUnit = null;
        }
    }

    //Target a unit.
    public void BattleTargetUnit(Unit u)
    {
        selectedUnit = u;
    }

    //Change the selected skill.
    public void SelectSkill(int i)
    {
        selectedSkill = i;
    }

    //This toggles auto mode.
    public void ToggleAuto()
    {
        if (auto) auto = false;
        else
        {
            auto = true;
            ManualUnit = null;
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
