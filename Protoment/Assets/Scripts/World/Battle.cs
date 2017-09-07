using UnityEngine;
using System.Collections;

public class Battle : MonoBehaviour
{
    //This class controls the flow of battle. Whos turn it is. How ATB ticks. When ATB ticks, and when battle is resolved.

    //A reference to our two arenas.
    public Arena PlayerArena;
    public Arena EnemyArena;

    //This controls battle speed
    public float speed = 1f;

    //On game updates
    public void Update()
    {
        //Tick the game forward.
        Tick();
    }

    //Tick the game update the battle state.
    public void Tick()
    {

    }

    //This goes off on Victory.
    public void Victory()
    {
        Debug.Log("You win the battle! Good job. I bet you're stuck at this screen with nothing to look at now, huh?");
    }

    //This goes off on Defeat.
    public void Defeat()
    {
        Debug.Log("You lost. Sucks to be you.");
    }
}
