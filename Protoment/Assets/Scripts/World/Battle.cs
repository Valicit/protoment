using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Battle : MonoBehaviour
{
    //This class controls the flow of battle. Whos turn it is. How ATB ticks. When ATB ticks, and when battle is resolved.

    //A reference to our two arenas.
    public Arena PlayerArena;
    public Arena EnemyArena;

    //A reference to the character whose turn is currently up.
    public static Unit readyUnit;
    public Unit selectedUnit;

    //A reference to our UI objects.
    public Button[] SkillButtons;
    public Text[] SkillButtonText;
    public int selectedSkill;
    public Button AutoButton;

    //These are battle variables.
    public static bool auto;
    public bool ended = false;
    public static long exp = 0;
    public static float speed = 0.5f;
    public static List<AttackData> usedActions = new List<AttackData>();
    public static Battle battle;

    //On start up.
    public void Start()
    {
        //Set the reference to the current battle to be this.
        battle = this;
    }

    //Update the battle.
    public void Update()
    {
        //Updates the units of the battle, if the battle is ongoing.
        if (PlayerArena.myParty.GetAllLiving().Count > 0 && EnemyArena.myParty.GetAllLiving().Count > 0) UpdateUnits();
        else if (PlayerArena.myParty.GetAllLiving().Count > 0 && !ended)
        {
            Invoke("NextWave", 2.0f);
            ended = true;
        }
        else if (!ended)
        {
            //Invoke("Deafeat", 2.0f);
            Defeat();
            ended = true;
        }

        //Update the battle UI.
            UpdateBattleUI();
    }

    //Update the battle UI.
    public void UpdateBattleUI()
    {
        UpdateSkillButtons();
    }

    //Update the skill buttons.
    public void UpdateSkillButtons()
    {
        //If the battle is set to auto.
        if (auto || readyUnit == null)
        {
            for (int i = 0; i < SkillButtons.Length; i++)
            {
                SkillButtons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            //If the battle is on manual and we have a unit waiting, show the buttons and update them.
            for (int i = 0; i < Mathf.Min(SkillButtons.Length, readyUnit.mySkills.Count); i++)
            {
                SkillButtons[i].gameObject.SetActive(true);
                SkillButtons[i].interactable = readyUnit.mySkills[i].IsReady();
                string t = readyUnit.mySkills[i].displayName;
                if (!readyUnit.mySkills[i].IsReady()) t += "CD" + readyUnit.mySkills[i].cd;
                SkillButtonText[i].text = t;
            }
        }
    }

    //This toggles auto mode.
    public void ToggleAuto()
    {
        if (auto) auto = false;
        else
        {
            auto = true;
        }
    }

    //Selects a skill, used by battle buttons.
    public void SelectSkill(int i)
    {
        selectedSkill = i;
    }

    //Target a unit in battle, while it's on manual.
    public void BattleTargetUnit(Unit u)
    {
        //If theres a ready unit.
        if (readyUnit != null)
        {
            selectedUnit = u;
        }
    }


    #region Units and Battle
    //Update the units of battle.
    public void UpdateUnits()
    {
        //If there are not any units ready.
        if (PlayerArena.myParty.GetAllReady().Count == 0 && EnemyArena.myParty.GetAllReady().Count == 0 && readyUnit == null)
        {
            //Tick the battle forward.
            Tick();
        }
        else if (readyUnit == null)
        {
            //Get whichever unit is ready, and make it the unit waiting to use its turn.
            if (PlayerArena.myParty.GetAllReady().Count > 0) readyUnit = PlayerArena.myParty.GetAllReady()[0];
            else
            {
                readyUnit = EnemyArena.myParty.GetAllReady()[0];
            }

            readyUnit.TurnStart();
        }
        else //If there IS a unit ready to take its turn, try to make a move.
        {
            MakeMove();
        }
    }

    //Tick the battle forward.
    public void Tick()
    {
        PlayerArena.Tick();
        EnemyArena.Tick();
    }

    //Attempt to make a move, but fail if not all data is available yet.
    public void MakeMove()
    {
        //Information a skill needs to go off. 1) A reference to the unit using the skill. 2) A reference to the characters party. 3) A reference to the enemy party. 
        //We also need to choose a skill to use. 
        //One auto, the skill will use an enum to find a random valid target from Party class. On manual, it will check if the player selected target is contained in that set of valid targets, and if not, the target will become unselected and the skill will not go off.
        
        //If we're on manual, and there's already a ready unit, a selected unit, and this isn't an enemy unit.
        if (!auto && readyUnit != null && selectedUnit != null && !EnemyArena.myParty.GetAllLiving().Contains(readyUnit))
        {
            //Get a reference to the selected skill.
            Skill s = readyUnit.mySkills[selectedSkill];

            //If there's a selected skill ready to be used.
            if (s.IsReady() && !s.isPassive)
            {
                //Gather attack data.
                AttackData data = new AttackData
                {
                    actor = readyUnit
                };
                if (PlayerArena.myParty.GetAllUnits().Contains(readyUnit))
                {
                    data.actorParty = PlayerArena.myParty;
                    data.defendingParty = EnemyArena.myParty;
                }
                else
                {
                    data.actorParty = EnemyArena.myParty;
                    data.defendingParty = PlayerArena.myParty;
                }

                //If we've got a valid target, use the skill.
                if (s.IsValidTarget(selectedUnit, data))
                {
                    //Use the skill.
                    data.selectedUnit = selectedUnit;
                    s.UseSkill(data);
                    selectedUnit = null;
                    selectedSkill = 0;
                    readyUnit.TurnEnd();
                    readyUnit = null;
                }
                else
                {
                    //Deselect the selected unit.
                    selectedUnit = null;
                }
            }
        }
        else if ((auto || EnemyArena.myParty.GetAllLiving().Contains(readyUnit)) && readyUnit != null) //If we're on auto, or this is an enemy unit, this happens instead.
        {
            //for each skill, counting down.
            for (int sk = readyUnit.mySkills.Count -1; sk >= 0; sk--)
            {
                Debug.Log("TEST");
                Skill autoSkill = readyUnit.mySkills[sk];

                //If the skill is ready and not passive.
                if (autoSkill.IsReady() && !autoSkill.isPassive)
                {
                    //Gather attack data.
                    AttackData data = new AttackData
                    {
                        actor = readyUnit
                    };
                    if (PlayerArena.myParty.GetAllUnits().Contains(readyUnit))
                    {
                        data.actorParty = PlayerArena.myParty;
                        data.defendingParty = EnemyArena.myParty;
                    }
                    else
                    {
                        data.actorParty = EnemyArena.myParty;
                        data.defendingParty = PlayerArena.myParty;
                    }

                    //Use the skill.
                    autoSkill.UseSkill(data);
                    readyUnit.TurnEnd();
                    readyUnit = null;
                    break;
                }
            }
        }
    }
    #endregion

    //This sets off the next wave.
    public void NextWave()
    {
        //Add exp.
        exp += EnemyArena.myParty.GetEXP();
        GrantExp();

        if (Player.currentDungeon.currentWave < Player.currentDungeon.waves.Length -1)
        {
            Player.currentDungeon.currentWave++;
            SceneManager.LoadScene("Battle");
        }
        else
        {
            Victory();
        }
    }

    //This goes off on Victory.
    public void Victory()
    {
        ResolveBattle();
        Debug.Log("You win the battle! Good job. I bet you're stuck at this screen with nothing to look at now, huh?");
        SceneManager.LoadScene("Town");
    }

    //This goes off on Defeat.
    public void Defeat()
    {
        ResolveBattle();
        Debug.Log("You lost. Sucks to be you.");
        SceneManager.LoadScene("Town");
    }

    //Do some battle end stuff.
    public void ResolveBattle()
    {
        //For each unit in the player's inventory.
        foreach (Unit u in Player.playerUnits)
        {
            u.cHP = u.GetmHP();
            u.myStatusEffects = new List<StatusEffect>();
            u.atb = 0;
        }

        //Remove exp.
        exp = 0;
    }

    //Grant exp.
    public void GrantExp()
    {
        //Get final exp.
        long fExp = 0;
        if (PlayerArena.myParty.GetAllUnits().Count > 0) fExp = (long)Mathf.Round((float)exp / PlayerArena.myParty.GetAllUnits().Count);

        //Give out exp.
        foreach (Unit u in Player.playerParty.GetAllLiving())
        {
            u.AddExp(fExp);
        }
    }
}
