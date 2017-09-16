using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitStatusScreen : MonoBehaviour
{
    //This shows all the information for one unit in a nice and easy place.

    //This is our unit.
    public Unit myUnit;

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

    //On updates. show unit info.
    public void Update()
    {
        if (myUnit != null) UpdateUI();
    }

    //Update ui.
    public void UpdateUI()
    {
        unitPort.sprite = myUnit.uSprite;
        txt_name.text = myUnit.uName;
        txt_class.text = myUnit.job;
        txt_level.text = "Lv. " + myUnit.level + " " + myUnit.exp + " / " + myUnit.GetENext(myUnit.level) + " " + ((float)myUnit.exp / (float)myUnit.GetENext(myUnit.level)) + "%";
        txt_HP.text = "HP: " + myUnit.GetmHP() + " (" + (myUnit.modHP * 100) + "%)";
        txt_str.text = "STR: " + myUnit.GetSTR() + " (" + (myUnit.modSTR * 100) + "%)";
        txt_def.text = "DEF: " + myUnit.GetDEF() + " (" + (myUnit.modDEF * 100) + "%)";
        txt_int.text = "INT: " + myUnit.GetINT() + " (" + (myUnit.modINT * 100) + "%)";
        txt_spr.text = "SPR: " + myUnit.GetSPR() + " (" + (myUnit.modSPR * 100) + "%)";
        txt_dex.text = "DEX: " + myUnit.GetDEX() + " (" + (myUnit.modDEX * 100) + "%)";
        txt_agi.text = "AGI: " + myUnit.GetAGI() + " (" + (myUnit.modAGI * 100) + "%)";
        txt_crit.text = "Crit: " + (myUnit.GetCrit()) + "%";
        txt_critDMG.text = "CritDMG: " + (myUnit.GetCritDMG() * 100) + "%";
        txt_speed.text = "Speed: " + myUnit.GetSpeed();
        txt_skill1.text = "SkillText";
        txt_skill2.text = "SkillText";
        txt_skill3.text = "SkillText";
    }

    //Select a new unit.
    public void OnUnitSelectButtonPressed(Unit u)
    {
        myUnit = u;
    }
}
