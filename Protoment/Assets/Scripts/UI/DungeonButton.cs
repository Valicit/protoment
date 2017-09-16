using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DungeonButton : MonoBehaviour
{
    //This is the dungeon this button begins.
    public Dungeon myDungeon;

    //This button sends the dungeon upwards.
    public void OnPress()
    {
        SendMessageUpwards("SelectDungeon", myDungeon);
    }
}
