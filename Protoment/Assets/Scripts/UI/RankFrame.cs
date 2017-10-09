using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankFrame : MonoBehaviour
{
    public Image[] rankIcons;
    public Sprite filledIcon;
    public Sprite emptyIcon;

    //Update the frame.
    public void UpdateFrame(int stars)
    {
        //Set rank icons.
        for (int i = 0; i < 10; i++)
        {
            if (stars > i) rankIcons[i].sprite = filledIcon;
            else rankIcons[i].sprite = emptyIcon;
        }
    }
}
