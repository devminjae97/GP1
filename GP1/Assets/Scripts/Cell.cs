using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public Vector2Int pos;
    public Vector2 posWorld;
    public bool isChecked;
    public int id;
    public bool[] doors = new bool[] { false, false, false, false };
    public HashSet<int> injectedCellList = new HashSet<int>();

    public Cell(Vector2Int pos, Vector2 posWorld)
    {
        this.pos = pos;
        this.posWorld = posWorld;
    }
    
    public bool IsInjected(int id)
    {
        return injectedCellList.Contains( id );
    }
}
