using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Cell : MonoBehaviour
{
    public bool isChecked;
    public bool isVisited;
    public int id;
    public Vector2Int pos;
    public Vector2 posWorld;
    private SpriteRenderer minimapSprite;
    private Color activeColor;
    private Color deactiveColor;
    public Dictionary<int, GameObject> walls;

    public Cell( Vector2Int pos, Vector2 posWorld )
    {
        this.pos = pos;
        this.posWorld = posWorld;
    }

    private void Awake()
    {
        minimapSprite = transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>();
        walls = new Dictionary<int, GameObject>
        {
            { 0, null },
            { 1, null },
            { 2, null },
            { 3, null }
        };
    }

    public void InitPos( Vector2Int pos )
    {
        this.pos = pos;

    }

    public void InitMinimapSprite( Sprite _sprite, Quaternion _rotate )
    {
        minimapSprite.sprite = _sprite;
        minimapSprite.transform.localRotation = _rotate;

        // 처음에는 투명하게 설정
        Color curColor = minimapSprite.color;
        curColor.a = 0;
        minimapSprite.color = curColor;
    }

    public void VisitCell()
    {
        isVisited = true;
        SetCameraPos( id );

        foreach (Cell curCell in DungeonManager.GetInstance().roomDic[id])
        {
            curCell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
            foreach (TileBase tb in curCell.GetComponentsInChildren<TileBase>())
            {
                tb.SetVisibility( true );
            }
        }
    }

    public void EnterCell( Cell nextCell )
    {
        nextCell.isVisited = true;
        SetCameraPos( nextCell.id );
        foreach (Cell cell in DungeonManager.GetInstance().roomDic[id])
        {
            cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = deactiveColor;
            foreach (TileBase tb in cell.GetComponentsInChildren<TileBase>())
            {
                tb.SetVisibility( false );
            }
        }

        foreach (Cell cell in DungeonManager.GetInstance().roomDic[nextCell.id])
        {
            cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
            foreach (TileBase tb in cell.GetComponentsInChildren<TileBase>())
            {
                tb.SetVisibility( true );
            }
        }
    }

    public void SetVisibleTiles( Cell cell, bool isVisible )
    {
        if (GameTestManager.GetInstance().allMapVisibleMode)
            return;

        foreach (Cell cur in DungeonManager.GetInstance().roomDic[cell.id])
        {
            cur.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
            foreach (TileBase tb in cur.GetComponentsInChildren<TileBase>())
            {
                tb.SetVisibility( isVisible );
            }
        }
    }

    public void SetCameraPos( int id )
    {
        if (GameTestManager.GetInstance().DoNotMoveCameraMode)
            return;

        DungeonManager.GetInstance().SetPlayerRoomID( id );
        DungeonManager.GetInstance().SetMainCameraPos();
    }
}
