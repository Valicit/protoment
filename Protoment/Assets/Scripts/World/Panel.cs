using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour
{
    //The panel will have references to the Unit standing on it, as well as all of the UI elements and such that it needs to display everything that happens.

    //This is the unit currently standing on this panel.
    public Unit myUnit;

    //This is a reference to the sprite that represents the character.
    public SpriteRenderer uSprite;
}
