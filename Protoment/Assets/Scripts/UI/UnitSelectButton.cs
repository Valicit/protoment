using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitSelectButton : MonoBehaviour {

    //This is a reference to our Unit.
    public Unit myUnit;

    //This is the unit image we should be changing.
    public Image unitImage;
    public RankFrame rankFrame;
    public Text txt_level;

    //This is what happens if we are a party button.
    public int x;
    public int y;
    public DungeonSelect dSelect;
    public void OnPressParty()
    {
        dSelect.PartySelectButton(x, y);
    }

    //This is what happens when a unit button is pressed.
    public void OnPress()
    {
        SendMessageUpwards("OnUnitSelectButtonPressed", myUnit, SendMessageOptions.DontRequireReceiver);
    }

    //Set the sprite for our image.
    public void SetImage(Sprite s)
    {
        unitImage.sprite = s;
    }
}
