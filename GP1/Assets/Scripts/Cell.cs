using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell : MonoBehaviour
{
    public bool isChecked;
    public bool isVisited;
    public int id;
    public Vector2Int pos;
    public Vector2 posWorld;
    public GameObject[,] tiles;
    public SpriteRenderer minimapSprite;
    public HashSet<Cell> cellsSameRoom;
    public Color activeColor;
    public Color deactiveColor;

    public Cell(Vector2Int pos, Vector2 posWorld)
    {
        this.pos = pos;
        this.posWorld = posWorld;
    }

    private void Awake()
    {
        minimapSprite = GetComponentInChildren<SpriteRenderer>();
        cellsSameRoom = new HashSet<Cell>();
    }

    public void InitPos( Vector2Int pos, Vector2 posWorld )
    {
        this.pos = pos;
        this.posWorld = posWorld;
    }

    public void InitMinimapSprite(Sprite _sprite, Quaternion _rotate)
    {
        minimapSprite.sprite = _sprite;
        minimapSprite.transform.transform.rotation = _rotate;

        // 처음에는 투명하게 설정
        Color curColor = minimapSprite.color;
        curColor.a = 0;
        minimapSprite.color = curColor;
    }
    
    public void VisitCell()
    {
        isVisited = true;

        foreach (Cell cell in cellsSameRoom)
        {
            cell.minimapSprite.color = activeColor;
            SetVisibleTiles( cell, true );
        }
    }

    public void EnterCell( Cell nextCell )
    {
        nextCell.isVisited = true;

        foreach (Cell cell in cellsSameRoom)
        {
            cell.minimapSprite.color = deactiveColor;
            SetVisibleTiles( cell, false );
        }
        foreach (Cell cell in nextCell.cellsSameRoom)
        {
            cell.minimapSprite.color = activeColor;
            SetVisibleTiles( cell, true );
        }
    }

    public void SetVisibleTiles( Cell cell, bool isVisible )
    {
        if (GameTestManager.GetInstance().allMapVisibleMode) 
            return;

        foreach (GameObject tile in cell.tiles)
        {
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            sr.enabled = isVisible;
        }
    }
}
