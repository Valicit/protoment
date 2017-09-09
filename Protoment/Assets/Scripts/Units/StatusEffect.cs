using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffect : ScriptableObject
{
    //This is an inheritable class for creating status effects that can be put on characters.

    //This is the Status Name.
    public string statusName;

    //This is the icon the status effect shows when it's on the character.
    public Sprite statusIcon;

    //this is basic information.
    public bool isStackable;
    public int duration;
    public bool preventAction;
}
