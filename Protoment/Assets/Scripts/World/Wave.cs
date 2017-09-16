using UnityEngine;
using System.Collections;

[System.Serializable]
public class Wave
{
    //This contains all the info for one wave of a dungeon.

    //These are all our units, written this way to make it easier in the editor, even though code wise it's silly.
    public UnitData unit00;
    public UnitData unit01;
    public UnitData unit02;
    public UnitData unit10;
    public UnitData unit11;
    public UnitData unit12;
    public UnitData unit20;
    public UnitData unit21;
    public UnitData unit22;
    public int[] Level = new int[9];
}
