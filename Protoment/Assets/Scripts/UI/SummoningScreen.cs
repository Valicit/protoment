using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SummoningScreen : MonoBehaviour {

    //These are references to ui objects.
    public Button btn_Basic;
    public Text txt_BasicButton;
    public Text txt_AdvancedButton;
    public Text txt_RainbowButton;
    public Text txt_LuminousButton;
    public Text txt_LightShadowButton;

    //This is a list of every summonable unit in the game.
    public List<UnitData> SummonableUnits;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Update the UI
        UpdateUI();
	}

    //Update the UI elements.
    public void UpdateUI()
    {
        txt_BasicButton.text = string.Format("Basic Summon: {0} / {1}", Player.basicMaterial, 100);
        txt_AdvancedButton.text = string.Format("Advanced Summon: {0} / {1}", Player.advancedMaterial, 100);
        txt_RainbowButton.text = string.Format("Rainbow Summon: {0} / {1}", Player.rainbowMaterial, 100);
        txt_LuminousButton.text = string.Format("Luminous Summon: {0} / {1}", Player.luminousMaterial, 100);
        txt_LightShadowButton.text = string.Format("Light and Shadow Summon: {0} / {1}", Player.lightShadowMaterial, 100);
    }

    //Perform a basic summon.
    public void BasicSummon()
    {
        Summon(new float[] { 2, 25, 100 }, new Rarity[] { Rarity.Rare, Rarity.Uncommon, Rarity.Common }, new List<Element>() { Element.Fire, Element.Water, Element.Wood, Element.Electric }, ref Player.basicMaterial);
    }

    //Perform an advanced summon.
    public void AdvancedSummon()
    {
        Summon(new float[] { 2, 25, 100 }, new Rarity[] { Rarity.Rare, Rarity.Rare, Rarity.Rare }, new List<Element>() { Element.Fire, Element.Water, Element.Wood, Element.Electric }, ref Player.advancedMaterial);
    }

    //Perform a rainbow summon.
    public void RainbowSummon()
    {
        Summon(new float[] { 2, 25, 100 }, new Rarity[] { Rarity.Rare, Rarity.Uncommon, Rarity.Common }, new List<Element>() { Element.Fire, Element.Water, Element.Wood, Element.Electric, Element.Light, Element.Dark }, ref Player.rainbowMaterial);
    }

    //Perform a luminous summon.
    public void LuminousSummon()
    {
        Summon(new float[] { 2, 25, 100 }, new Rarity[] { Rarity.Rare, Rarity.Rare, Rarity.Rare }, new List<Element>() { Element.Fire, Element.Water, Element.Wood, Element.Electric, Element.Light, Element.Dark }, ref Player.luminousMaterial);
    }

    //Perform a light and shadow summon.
    public void LightShadowSummon()
    {
        Summon(new float[] { 2, 25, 100 }, new Rarity[] { Rarity.Rare, Rarity.Rare, Rarity.Rare }, new List<Element>() { Element.Light, Element.Dark }, ref Player.lightShadowMaterial);
    }

    //Perform a basic summon.
    public void Summon(float[] chances, Rarity[] rares, List<Element> elements, ref int resource)
    {
        //If the player has the resources.
        if (resource >= 100)
        {
            //Pay the cost.
            resource -= 100;

            //Roll a random number between 0 and 100.
            float r = Random.Range(0, 100);

            //Summon based on the result.
            for (int i = 0; i < chances.Length; i++)
            {
                if (r < chances[i])
                {
                    SummonUnit(SummonableUnits.FindAll(n => n.uRarity == rares[i] && elements.Contains(n.uElement)));
                    break;
                }
            }
        }
    }

    //Summon a unit from the specified list.
    private void SummonUnit(List<UnitData> r)
    {
        //Get our unit.
        Unit n = Unit.NewUnit(r[Random.Range(0, r.Count)]);
        Player.playerUnits.Add(n);
        Debug.Log(string.Format("Got {0}! Rarity: {1}", n.uName, n.uRarity));
    }
}
