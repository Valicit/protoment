﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitStatusScreen : MonoBehaviour
{
    //This shows all the information for one unit in a nice and easy place.

    //This is our unit.
    public Unit myUnit;

    //This is a reference to the party select.
    public PartyUnitSelect partyPane;

    //This is all the UI objects.
    public Image unitPort;
    public Text txt_name;
    public Text txt_class;
    public Text txt_level;
    public Text txt_HP;
    public Text txt_str;
    public Text txt_def;
    public Text txt_int;
    public Text txt_spr;
    public Text txt_dex;
    public Text txt_agi;
    public Text txt_crit;
    public Text txt_critDMG;
    public Text txt_speed;
    public Text txt_skill1;
    public Text txt_skill2;
    public Text txt_skill3;

    //These are button texts.
    public Text txt_reap;
    public Text txt_upgrade;

    //On updates. show unit info.
    public void Update()
    {
        if (myUnit != null) UpdateUI();
        else myUnit = Player.playerUnits[0];
    }

    //Update ui.
    public void UpdateUI()
    {
        unitPort.sprite = myUnit.uSprite;
        txt_name.text = myUnit.uName;
        txt_class.text = myUnit.job;
        txt_level.text = "Lv. " + myUnit.level + " / " + MathP.maxLevels[myUnit.rank - 1];// + " " + myUnit.exp + " / " + myUnit.GetENext(myUnit.level) + " " + ((float)myUnit.exp / (float)myUnit.GetENext(myUnit.level)) + "%";
        txt_HP.text = "HP: " + myUnit.GetmHP();
        txt_str.text = "STR: " + myUnit.GetSTR();
        txt_def.text = "DEF: " + myUnit.GetDEF();
        txt_int.text = "INT: " + myUnit.GetINT();
        txt_spr.text = "SPR: " + myUnit.GetSPR();
        txt_dex.text = "DEX: " + myUnit.GetDEX();
        txt_agi.text = "AGI: " + myUnit.GetAGI();
        txt_crit.text = "Crit: " + (myUnit.GetCrit()) + "%";
        txt_critDMG.text = "CritDMG: " + (myUnit.GetCritDMG() * 100) + "%";
        txt_speed.text = "Speed: " + myUnit.GetSpeed();
        if (myUnit.GetMySkillsFull().Count > 0) txt_skill1.text = string.Format("{0}: {1} (Unlocks at rank {2})", myUnit.GetMySkillsFull()[0].displayName, myUnit.GetMySkillsFull()[0].description, myUnit.GetMySkillsFull()[0].rankUnlock);
        else txt_skill1.text = "";
        if (myUnit.GetMySkillsFull().Count > 1) txt_skill2.text = string.Format("{0}: {1} (Unlocks at rank {2})", myUnit.GetMySkillsFull()[1].displayName, myUnit.GetMySkillsFull()[1].description, myUnit.GetMySkillsFull()[1].rankUnlock);
        else txt_skill2.text = "";
        if (myUnit.GetMySkillsFull().Count > 2) txt_skill3.text = string.Format("{0}: {1} (Unlocks at rank {2})", myUnit.GetMySkillsFull()[2].displayName, myUnit.GetMySkillsFull()[2].description, myUnit.GetMySkillsFull()[2].rankUnlock);
        else txt_skill3.text = "";

        txt_reap.text = string.Format("Reap: {0}", 1);
        txt_upgrade.text = string.Format("Upgrade: {0} / {1}", Player.imagination[myUnit.rank - 1], myUnit.rank);
    }

    //Select a new unit.
    public void OnUnitSelectButtonPressed(Unit u)
    {
        myUnit = u;
    }

    //Delete a unit.
    public void DeleteUnit()
    {
        //If there is a unit selected.
        if (myUnit != null && Player.playerUnits.Count > 1)
        {
            Player.playerUnits.Remove(myUnit);
            Player.playerParty.Delete(myUnit);
            myUnit = Player.playerUnits[0];
        }
        partyPane.OnLoad();
    }

    //Upgrade the unit.
    public void OnUpgrade()
    {
        //If we have the mana.
        if (Player.imagination[myUnit.rank - 1] >= myUnit.rank && myUnit.level >= MathP.maxLevels[myUnit.rank - 1])
        {
            //Remove the mana.
            Player.imagination[myUnit.rank - 1] -= myUnit.rank;

            //Return the unit to level 1.
            myUnit.level = 1;
            myUnit.exp = 0;
            myUnit.mHP = myUnit.myData.HP;
            myUnit.cHP = myUnit.GetmHP();
            myUnit.STR = myUnit.myData.STR;
            myUnit.DEF = myUnit.myData.DEF;
            myUnit.INT = myUnit.myData.INT;
            myUnit.SPR = myUnit.myData.SPR;
            myUnit.DEX = myUnit.myData.DEX;
            myUnit.AGI = myUnit.myData.AGI;

            //Increase rank.
            myUnit.rank++;
        }
    }

    //Reap the unit.
    public void OnReap()
    {
        if (myUnit.level >= MathP.maxLevels[myUnit.rank - 1])
        {
            //Add the reaped mana.
            myUnit.Reap();

            //Delete the unit.
            DeleteUnit();
        }
    }
}
