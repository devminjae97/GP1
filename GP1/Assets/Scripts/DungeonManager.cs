using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct FTileInfoByCellID
{
    public Tilemap tilemap;
    public Vector3Int pos;

    public FTileInfoByCellID(Tilemap _tilemap, Vector3Int _pos)
    {
        tilemap = _tilemap;
        pos = _pos;
    }
};

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager instance;

    public int playerRoomID;
    public Player player;
    public Camera mainCamera;
    public Camera minimapCamera;
    public Dictionary<int, HashSet<Cell>> roomDic;
    public Cell[,] cellList;
    public Tilemap tilemap;
    public Dictionary<int, List<FTileInfoByCellID>> tilemapDic;
    public Dictionary<int, List<Door>> doorDic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy( gameObject );
        }

        roomDic = new Dictionary<int, HashSet<Cell>>();
        tilemapDic = new Dictionary<int, List<FTileInfoByCellID>>();
        doorDic = new Dictionary<int, List<Door>>();
    }

    public static DungeonManager GetInstance()
    {
        return instance;
    }

    public void AddToRoomDic(Cell cell)
    {
        if (roomDic.ContainsKey(cell.id ))
        {
            roomDic[cell.id].Add( cell );
        }
        else
        {
            roomDic.Add( cell.id, new HashSet<Cell>() { cell } );
        }
    }

    public void SetPlayerRoomID(int id)
    {
        playerRoomID = id;
    }

    public void SetPlayerPos(Vector3Int pos)
    {
        player.transform.position = tilemap.CellToWorld(pos);
    }

    public void SetMainCameraPos()
    {
        Vector3 pos = new Vector3( 0, 0, 0 );

        if (!roomDic.ContainsKey( playerRoomID ))
            return;

        foreach (Cell cell in roomDic[playerRoomID])
        {
            pos += cell.transform.position;
        }

        pos /= roomDic[playerRoomID].Count;
        mainCamera.transform.position = new Vector3( pos.x, pos.y, -10 );
        minimapCamera.transform.position = new Vector3( pos.x, pos.y, -10 );
    }

    public void SetPlayerTransform(Vector2 pos, float size)
    {
        player.transform.position = pos;
        player.transform.localScale = new Vector3( size, size, 0 );
    }

    public void AddToTilemapDic( int id, Tilemap tilemapType, Vector3Int pos )
    {
        FTileInfoByCellID tileInfo = new FTileInfoByCellID( tilemapType, pos );
        if (tilemapDic.ContainsKey( id ))
        {
            tilemapDic[id].Add( tileInfo );
        }
        else
        {
            tilemapDic.Add( id, new List<FTileInfoByCellID>() { tileInfo } );
        }
    }

    public void AddToDoorDic( int id, Door door )
    {
        if (doorDic.ContainsKey( id ))
        {
            doorDic[id].Add( door );
        }
        else
        {
            doorDic.Add( id, new List<Door>() { door } );
        }
    }

    public void SetVisibilityTiles(int id, bool isVisible)
    {
        if (tilemapDic.ContainsKey(id))
        { 
            foreach (FTileInfoByCellID tileInfo in tilemapDic[id])
            {
                Tilemap tilemapType = tileInfo.tilemap;
                Vector3Int pos = tileInfo.pos;

                Color color = tilemapType.GetColor( pos );
                if (isVisible)
                {
                    color.a = 1;
                }
                else
                {
                    color.a = 0;
                }
                tilemapType.SetColor( pos, color );
            }

            foreach (Door door in doorDic[id])
            {
                door.SetVisibility( isVisible );
                if (isVisible)
                {
                    door.GetComponent<BoxCollider2D>().isTrigger = true;
                }
                else
                {
                    door.GetComponent<BoxCollider2D>().isTrigger = false;
                }
            }
        }
    }

    public void ActivateMinimap( int id, bool isActivate )
    {
        foreach(Cell cell in roomDic[id])
        {
            SpriteRenderer minimapRenderer = cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>();
            cell.isVisited = true;
            if (isActivate)
            {
                minimapRenderer.color = cell.activeColor;
            }
            else
            {
                minimapRenderer.color = cell.deactiveColor;
            }
        }
    }

    public void SetVisibilityMinimap( int id, bool isActivate )
    {
        foreach (Cell cell in roomDic[id])
        {
            SpriteRenderer minimapRenderer = cell.transform.Find( "minimapSprite" ).GetComponent<SpriteRenderer>();
            if (isActivate)
            {
                Color color = minimapRenderer.color;
                color.a = 1;
                minimapRenderer.color = color;
            }
            else
            {
                Color color = minimapRenderer.color;
                color.a = 0;
                minimapRenderer.color = color;
            }
        }
    }
}
