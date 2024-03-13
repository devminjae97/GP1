using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public bool isChecked;
    public int id;
    public Vector2Int pos;
    public Vector2 posWorld;
    public GameObject[,] tiles;

    public Cell(Vector2Int pos, Vector2 posWorld)
    {
        this.pos = pos;
        this.posWorld = posWorld;
    }
}
