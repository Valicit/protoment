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
    public static Unit triggerUnit;
    public Unit selectedUnit;

    //A reference to our UI objects.
    public Button[] SkillButtons;
    public Text[] SkillButtonText;
    public int selectedSkill;
    public Button AutoButton;
    public GameObject EscapeScreen;
    public Text txt_EscapeRewardText;
    public GameObject DefeatScreen;
    public GameObject VictoryScreen;
    public Text VictoryText;
    public Image VictorySprite;
    public Text WaveText;

    //These are battle variables.
    public static bool auto;
    public bool ended = false;
    public static long exp = 0;
    public static float speed = 1000f;
    public float wait = 0f;
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
        if (wait > 0)
        {
            wait -= Time.deltaTime;
        }
        else if (PlayerArena.myParty.GetAllLiving().Count > 0 && EnemyArena.myParty.GetAllLiving().Count > 0)
        {
            UpdateUnits();
        }
        else if (PlayerArena.myParty.GetAllLiving().Count > 0 && !ended)
        {
            NextWave();
            ended = true;
        }
        else if (!ended)
        {
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
        WaveText.text = string.Format("Wave: {0} / {1}", Player.currentDungeon.currentWave + 1, Player.currentDungeon.waves.Length);
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
            for (int i = 0; i < Mathf.Min(SkillButtons.Length, readyUnit.GetMySkills().Count); i++)
            {
                SkillButtons[i].gameObject.SetActive(true);
                SkillButtons[i].interactable = readyUnit.GetMySkills()[i].IsReady();
                string t = readyUnit.GetMySkills()[i].displayName;
                if (!readyUnit.GetMySkills()[i].IsReady()) t += "CD" + readyUnit.GetMySkills()[i].cd;
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

    //Get the average agility stat.
    public float GetAverageAgi()
    {
        List<Unit> living = new List<Unit>();
        living.AddRange(PlayerArena.myParty.GetAllLiving());
        living.AddRange(EnemyArena.myParty.GetAllLiving());
        float avg = 0;
        foreach (Unit u in living)
        {
            avg += u.GetAGI();
        }
        avg = avg / living.Count;
        return avg;
    }

    #region Making a Move
    //Attempt to make a move, but fail if not all data is available yet.
    public void MakeMove()
    {
        //Make sure our ready unit isn't dead. That happens sometimes.
        CheckLivingReadyUnit();

        //If we're on manual, and there's already a ready unit, a selected unit, and this isn't an enemy unit.
        if (!auto && readyUnit != null && selectedUnit != null && !EnemyArena.myParty.GetAllLiving().Contains(readyUnit) && readyUnit.myStatusEffects.Find(n => n.preventAction || n.provoke) == null)
        {
            //Execute manual code.
            MoveManual();
        }
        else if ((auto || (EnemyArena.myParty.GetAllLiving().Contains(readyUnit)) && readyUnit != null)) //If we're on auto, or this is an enemy unit, this happens instead.
        {
            //Execute Auto code.
            MoveAuto();
        }
    }

    //Make a manual move.
    public void MoveManual()
    {
        //If we aren't stunned or something.
        if (readyUnit.myStatusEffects.Find(n => n.preventAction) == null)
        {
            AttackData data = GatherAttackData();

            //Set our skill.
            Skill s = SetSkill(data);

            //Use our skill.
            UseSkill(s, data);
        }
        else EndTurn();
    }

    //Make an automatic move.
    public void MoveAuto()
    {
        //If we aren't stunned or something.
        if (readyUnit.myStatusEffects.Find(n => n.preventAction) == null)
        {
            AttackData data = GatherAttackData();
            Skill s = SetSkill(data);
            UseSkill(s, data);
        }
        else
        {
            EndTurn();
        }
    }

    //Gather attack data.
    public AttackData GatherAttackData()
    {
        Unit u = readyUnit;
        if (triggerUnit != null) u = triggerUnit;

        //Gather attack data.
        AttackData data = new AttackData
        {
            actor = u
        };
        if (PlayerArena.myParty.GetAllUnits().Contains(u))
        {
            data.actorParty = PlayerArena.myParty;
            data.defendingParty = EnemyArena.myParty;
        }
        else
        {
            data.actorParty = EnemyArena.myParty;
            data.defendingParty = PlayerArena.myParty;
        }
        return data;
    }

    //Use a skill.
    public bool UseSkill(Skill s, AttackData data)
    {
        //If the skill is ready and not passive.
        if (s.IsReady() && !s.isPassive)
        {
            //If this is auto or our selected target is vald, use the skill. If we're provoked, ignore validation.
            if (auto || s.IsValidTarget(selectedUnit, data) || s.isTriggered || data.actor.myStatusEffects.Find(n => n.provoke) != null || EnemyArena.myParty.GetAllLiving().Contains(data.actor))
            {
                //If we are provoked, the applier exists and is alive, select that guy.
                SetSkillTarget(data);

                //Use the skill.
                if (data.actor.myStatusEffects.Find(n => n.preventAction) == null) s.UseSkill(data);
                if (!s.isTriggered)
                {
                    EndTurn();
                }
                return true;
            }
            else
            {
                selectedUnit = null;
            }
        }
        return false;
    }

    //Check if the ready unit is dead, and if so end the turn.
    public void CheckLivingReadyUnit()
    {
        if (!readyUnit.IsAlive())
        {
            readyUnit = null;
        }
    }

    //Set the skill we're going to use.
    public Skill SetSkill(AttackData data)
    {
        Skill s = null;
        //If manual.
        if (!auto && !EnemyArena.myParty.GetAllLiving().Contains(readyUnit))
        {
            //Get a reference to the selected skill.
            s = readyUnit.GetMySkills()[selectedSkill];
        }
        //Otherwise.
        else
        {
            //for each skill, counting down.
            for (int sk = readyUnit.GetMySkills().Count - 1; sk >= 0; sk--)
            {
                //Get the selected skill.
                if (readyUnit.GetMySkills()[sk].IsReady() && !readyUnit.GetMySkills()[sk].isPassive && readyUnit.GetMySkills()[sk].IsConditionMet(data))
                {
                    s = readyUnit.GetMySkills()[sk];
                    break;
                }
            }
        }

        //Return the result.
        if (readyUnit.myStatusEffects.Find(n => n.provoke) != null) s = readyUnit.GetMySkills()[readyUnit.myStatusEffects.Find(n => n.provoke).provokeSkill];
        return s;
    }

    //Check if we're provoked and change targets if so.
    public void SetSkillTarget(AttackData data)
    {
        //Choose a selected unit normally.
        if (selectedUnit != null) data.selectedUnit = selectedUnit;

        //Change it if we're provoked.
        if (data.actor.myStatusEffects.Find(n => n.provoke) != null)
            if (data.actor.myStatusEffects.Find(n => n.provoke).applier != null)
                if (data.actor.myStatusEffects.Find(n => n.provoke).applier.IsAlive()) data.selectedUnit = readyUnit.myStatusEffects.Find(n => n.provoke).applier;
    }

    //End the units turn.
    public void EndTurn()
    {
        selectedUnit = null;
        selectedSkill = 0;
        readyUnit.TurnEnd();
        readyUnit = null;
    }
    #endregion
    #endregion

    //This sets off the next wave.
    public void NextWave()
    {
        //Add exp.
        exp += EnemyArena.myParty.GetEXP();
        GrantExp();

        //If this is a random dungeon.
        if (Player.currentDungeon.isRandom && Player.currentDungeon.currentWave < Player.currentDungeon.waves.Length - 1 && (Player.currentDungeon.currentWave + 1) % 10 == 9)
        {
            Escape();
        }
        //Otherwise, if this isn't the end of the dungeon load the next wave.
        else if (Player.currentDungeon.currentWave < Player.currentDungeon.waves.Length -1)
        {
            Player.currentDungeon.currentWave++;
            SceneManager.LoadScene("Battle");
        }
        else
        {
            Victory();
        }
    }

    //This goes off when the player gets a chance to leave.
    public void Escape()
    {
        EscapeScreen.SetActive(true);
        txt_EscapeRewardText.text = Player.currentDungeon.GrantReward();
    }

    //This goes off on Victory.
    public void Victory()
    {
        VictoryScreen.SetActive(true);
        VictoryText.text = Player.currentDungeon.GrantReward();
    }

    //This goes off on Defeat.
    public void Defeat()
    {
        DefeatScreen.SetActive(true);        
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

            foreach (Skill s in u.GetMySkills())
            {
                s.cd = 0;
            }
        }

        //Remove exp.
        exp = 0;

        //Deselect characters.
        readyUnit = null;
    }

    //This happens when the done button is pressed.
    public void OnDoneButton()
    {
        //Resolve the battle and go back to town.
        ResolveBattle();
        SceneManager.LoadScene("Town");
    }

    //This is what happens when the repeat button is pressed.
    public void OnRepeatButton()
    {
        //Resolve the battle and repeat the stage.
        ResolveBattle();
        if (!Player.currentDungeon.isRandom) Player.currentDungeon.currentWave = 0;
        else Player.currentDungeon.currentWave = Player.currentDungeon.itemWorldEquip.level - 1;
        SceneManager.LoadScene("Battle");
    }

    //This is what happens when the continue button is pressed.
    public void OnContinueButton()
    {
        //Level up the item if there is one.
        Player.currentDungeon.currentWave++;
        SceneManager.LoadScene("Battle");
    }

    //On escape done button.
    public void OnEscapeButton()
    {
        //Level up the item if there is one.
        if (Player.currentDungeon.itemWorldEquip != null)
        {
            Equipment e = Player.currentDungeon.itemWorldEquip;
            e.SetLevel(Mathf.Max(e.level, Player.currentDungeon.currentWave + 2));
        }
        //Resolve the battle and go back to town.
        ResolveBattle();
        SceneManager.LoadScene("Town");
    }

    //Grant exp.
    public void GrantExp()
    {
        //Get final exp.
        long fExp = 0;
        if (PlayerArena.myParty.GetAllUnits().Count > 0) fExp = (long)Mathf.Round((float)exp / PlayerArena.myParty.GetAllUnits().Count);

        //Give out exp.
        foreach (Unit u in Player.playerParty.GetAllUnits())
        {
            u.AddExp(fExp);
        }
        exp = 0;
    }
}
