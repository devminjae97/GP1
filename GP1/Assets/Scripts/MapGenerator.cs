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
    // ������, ����, �Ʒ�, ��, ���ʾƷ�, �����ʾƷ�, ������, ��������
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
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(-1, 0) },              // ������ ��
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(1, 0) },              // ������ �Ʒ�
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, 1) },              // �Ʒ� ������
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, -1) },              // �Ʒ� ����
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(-1, 0) },              // ���� ��
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(1, 0) },              // ���� �Ʒ�
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, 1) },              // �� ������
            new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, -1) },              // �� ����          
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
