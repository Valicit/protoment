using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipSelectButton : MonoBehaviour {

    //This is our equipment.
    public Equipment myEquip;

    //These are references to button elements.
    public Text buttonText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Set the button text.
    public void SetText(string text)
    {
        buttonText.text = text;
    }

    //This is what happens when the button is pressed.
    public void ButtonPress()
    {
        SendMessageUpwards("OnInventorySelect", this.myEquip);
    }
}
