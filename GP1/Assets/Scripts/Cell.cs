using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    public UnityEngine.Color activeColor;
    public UnityEngine.Color deactiveColor;
    public Dictionary<int, GameObject> walls;
    public List<Door> doors;
    public Vector3Int tilemapLocalPos;
    //public CustomTile[,] tiles;

    public Cell( Vector2Int pos, Vector2 posWorld, int size )
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

        doors = new List<Door>();
    }

    public void InitCell( Vector2Int pos )
    {
        this.pos = pos;
    }

    public void InitMinimapSprite( Sprite _sprite, Quaternion _rotate )
    {
        minimapSprite.sprite = _sprite;
        minimapSprite.transform.localRotation = _rotate;
    }

    /*public void VisitCell()
    {
        isVisited = true;
        SetCameraPos( id );

        foreach (Cell curCell in DungeonManager.GetInstance().roomDic[id])
        {
            curCell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
        }
    }

    public void EnterCell( Cell nextCell )
    {
        nextCell.isVisited = true;
        SetCameraPos( nextCell.id );
        foreach (Cell cell in DungeonManager.GetInstance().roomDic[id])
        {
            cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = deactiveColor;
            cell.SetDoorsEnable( true );
        }

        foreach (Cell cell in DungeonManager.GetInstance().roomDic[nextCell.id])
        {
            cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
            cell.SetDoorsEnable( false );
        }
    }

    public void SetVisibleTiles( Cell cell, bool isVisible )
    {
        if (GameTestManager.GetInstance().allMapVisibleMode)
            return;

        foreach (Cell cur in DungeonManager.GetInstance().roomDic[cell.id])
        {
            cur.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>().color = activeColor;
        }
    }*/

    public void SetCameraPos( int id )
    {
        if (GameTestManager.GetInstance().DoNotMoveCameraMode)
            return;

        DungeonManager.GetInstance().SetPlayerRoomID( id );
        DungeonManager.GetInstance().SetMainCameraPos();
    }

    public void SetDoorsVisibility(bool isVisible)
    {
        foreach(Door curDoor in doors)
        {
            //curDoor
        }
    }

    public void SetDoorsEnable( bool isEnable )
    {
        foreach (Door curDoor in doors)
        {
            curDoor.enabled = isEnable;
        }
    }
}
