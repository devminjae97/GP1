using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public UnityEngine.Tilemaps.TileBase tilebase;
    private float tileSizePerCell;
    private float tileNumPerCell;
    private int roomCount = 1;
    // 오른쪽, 왼쪽, 아래, 위, 왼쪽아래, 오른쪽아래, 왼쪽위, 오른쪽위
    private int[] xdir = new int[] { 0, 0, 1, -1, 1, 1, -1, -1 };
    private int[] ydir = new int[] { 1, -1, 0, 0, -1, 1, -1, 1 };
    [SerializeField] private int roomDepth = 10;
    [SerializeField] private int cellSize = 30;
    private Dictionary<int, HashSet<int>> roomIDHash = new Dictionary<int, HashSet<int>>();
    public Vector2Int[][][] RoomTypes = new Vector2Int[][][]
    {
        new Vector2Int[][]
        {
            new Vector2Int[] { new Vector2Int( 0, 0 ) }
        },
        new Vector2Int[][]
        {
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ) },
        },
        new Vector2Int[][]
        {
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(-1, 0) },              // 오른쪽 위
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(1, 0) },              // 오른쪽 아래
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, 1) },              // 아래 오른쪽
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, -1) },              // 아래 왼쪽
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(-1, 0) },              // 왼쪽 위
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(1, 0) },              // 왼쪽 아래
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, 1) },              // 위 오른쪽
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, -1) },              // 위 왼쪽          
        },
        new Vector2Int[][]
        {
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1, 1), new Vector2Int(0, 1) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1,- 1), new Vector2Int(0, -1) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, 1), new Vector2Int(0, 1) },
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, -1), new Vector2Int(0, -1) }
        }
    };

    private void Start()
    {
        InitSetting();
    }

    void InitSetting()
    {
        tileSizePerCell = cellSize / tileNumPerCell;
    }
}
