using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager instance;

    public int playerRoomID;
    public Player player;
    public Camera mainCamera;
    public Camera minimapCamera;
    public Dictionary<int, HashSet<Cell>> roomDic;
    public Cell[,] cellList;
    
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

    public void SetMainCameraPos()
    {
        Vector2 pos = new Vector2( 0, 0 );

        if (!roomDic.ContainsKey( playerRoomID ))
            return;

        foreach (Cell cell in roomDic[playerRoomID])
        {
            pos += cell.posWorld;
        }

        pos /= roomDic[playerRoomID].Count;
        mainCamera.transform.position = new Vector3( pos.x, pos.y, -10 );
        minimapCamera.transform.position = new Vector3( pos.x, pos.y, -180 );
    }

    public void SetPlayerSize(float size)
    {
        player.transform.localScale = new Vector3( size, size, 0 );
    }
}
