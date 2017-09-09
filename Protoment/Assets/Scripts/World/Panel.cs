﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    //The panel will have references to the Unit standing on it, as well as all of the UI elements and such that it needs to display everything that happens.

    //This is the unit currently standing on this panel.
    public Unit myUnit;

    //These are references to bits of UI and other panel stuff.
    public SpriteRenderer uSprite;
    public SpriteRenderer pSprite;
    public GameObject uSpriteObject;
    public Canvas uCanvas;
    public Slider HPBar;
    public Slider ATBBar;

    //This is our text prefab.
    public GameObject textFab;

    //On start up.
    public void Start()
    {
        InvokeRepeating("UpdateFloatingText", 0.4f, 0.4f);
    }

    //Update the panel.
    public void Update()
    {
        //Update the sprite.
        UpdateSprite();

        //Update UI stuff.
        UpdateUnitUI();
    }

    //Update the sprite.
    public void UpdateSprite()
    {
        //If the unit is null, disable the sprite.
        if (myUnit == null)
        {
            uSpriteObject.SetActive(false);
            HPBar.gameObject.SetActive(false);
            ATBBar.gameObject.SetActive(false);
        }
        else
        {
            uSpriteObject.SetActive(true);
            HPBar.gameObject.SetActive(true);
            ATBBar.gameObject.SetActive(true);
        }
    }

    //Update the unit UI.
    public void UpdateUnitUI()
    {
        //If we've got a unit.
        if (myUnit != null)
        {
            HPBar.value = ((float)myUnit.cHP / (float)myUnit.GetmHP());
            ATBBar.value = myUnit.atb / 100;
            uSprite.sprite = myUnit.uSprite;
        }
    }

    //Update any floating text, or spawn more if it's not done yet.
    public void UpdateFloatingText()
    {
        //If we have a unit.
        if (myUnit != null)
        {
            //If we don't currently have active text and there is text in the queue.
            if (myUnit.textQueue.Count > 0)
            {
                GameObject tObject = GameObject.Instantiate(textFab, uCanvas.transform.position, uCanvas.transform.rotation, uCanvas.transform) as GameObject;
                Text dText = tObject.GetComponent<Text>();
                dText.text = myUnit.textQueue[0];
                dText.color = myUnit.textColor[0];
                myUnit.textQueue.RemoveAt(0);
                myUnit.textColor.RemoveAt(0);
            }
        }
    }
}
