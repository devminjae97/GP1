using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ETileType
{
    eNormal,
    eLeftUp,
    eRightUp,
    eLeftDown,
    eRightDown,
    eLeft,
    eRight,
    eUp,
    eDown,
    eDoor,
}

public class RoomTypeManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    private static RoomTypeManager instance;

    public Vector2Int[][] size1 = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int( 0, 0 ) }
    };
    public Vector2Int[][] size2 = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ) },
    };
    public Vector2Int[][] size3 = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(-1, 0) },              // ������ ��
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(1, 0) },              // ������ �Ʒ�
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, 1) },              // �Ʒ� ������
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, -1) },              // �Ʒ� ����
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(-1, 0) },              // ���� ��
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(1, 0) },              // ���� �Ʒ�
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, 1) },              // �� ������
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, -1) },              // �� ����          
    };         
    public Vector2Int[][] size4 = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1, 1), new Vector2Int(0, 1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1,- 1), new Vector2Int(0, -1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, 1), new Vector2Int(0, 1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, -1), new Vector2Int(0, -1) }
    };

    public Vector2Int[][][] RoomTypes = new Vector2Int[4][][];
    [SerializeField] private GameObject groundObj;
    [SerializeField] private GameObject wallObj;
    [SerializeField] private GameObject doorObj;
    [SerializeField] private Sprite leftUp;
    [SerializeField] private Sprite leftDown;
    [SerializeField] private Sprite rightUp;
    [SerializeField] private Sprite rightDown;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite door;
    [SerializeField] private int spriteSize = 10;
    [SerializeField] private Dictionary<ETileType, Sprite> spriteDic;
    SpriteRenderer spriteObj;
    [SerializeField] private GameObject groundParent;
    [SerializeField] private GameObject wallParent;
    [SerializeField] private GameObject doorParent;

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }
        else
        {
            Destroy( gameObject );
        }

        // RoomTypes �迭 �ʱ�ȭ
        RoomTypes[0] = size1; 
        RoomTypes[1] = size2; 
        RoomTypes[2] = size3; 
        RoomTypes[3] = size4;

        spriteDic = new Dictionary<ETileType, Sprite>() { 
            { ETileType.eLeftUp, leftUp },
            { ETileType.eRightUp, rightUp },
            { ETileType.eLeftDown, leftDown },
            { ETileType.eRightDown, rightDown },
            { ETileType.eLeft, left },
            { ETileType.eRight, right },
            { ETileType.eUp, up },
            { ETileType.eDown, down },
            { ETileType.eNormal, normal },
            { ETileType.eDoor, door },
        };
    }

    public static RoomTypeManager GetInstance()
    {
        return instance;
    }

    public int SpriteSize
    {
        get { return spriteSize; }
        set { spriteSize = value; }
    }

    public void DrawGround( ETileType tileType, GameObject pos, float scaleValue )
    {
        //GameObject curGroundObj = Instantiate( groundObj );
        pos.transform.parent = wallParent.transform;
        spriteObj = pos.GetComponent<SpriteRenderer>();
        spriteObj.sprite = spriteDic[tileType];
        spriteObj.transform.position = pos.GetComponent<TileBase>().posWorld;
        spriteObj.transform.localScale = new Vector2( scaleValue, scaleValue );
        spriteObj.sortingLayerName = "Tile";

        spriteObj.GetComponent<BoxCollider2D>().isTrigger = false;
        spriteObj.AddComponent<Tile>();
    }

    public void DrawWall( ETileType tileType, GameObject pos, float scaleValue )
    {
        spriteObj = pos.GetComponent<SpriteRenderer>();
        if (spriteObj.sprite == door) return;
        pos.transform.parent = wallParent.transform;
        spriteObj.sprite = spriteDic[tileType];
        spriteObj.transform.position = pos.GetComponent<TileBase>().posWorld;
        spriteObj.transform.localScale = new Vector2( scaleValue, scaleValue );
        spriteObj.sortingLayerName = "Tile";

        spriteObj.GetComponent<BoxCollider2D>().isTrigger = false;
        spriteObj.AddComponent<Wall>();
    }

    public void DrawDoor( ETileType tileType, GameObject tileBaseObj, float scaleValue, Color color )
    {
        spriteObj = tileBaseObj.GetComponent<SpriteRenderer>();
        spriteObj.transform.parent = doorParent.transform;
        spriteObj.sprite = door;
        spriteObj.transform.position = tileBaseObj.GetComponent<TileBase>().PosWorld;
        spriteObj.transform.localScale = new Vector2( scaleValue, scaleValue );
        spriteObj.color = color;
        spriteObj.sortingLayerName = "Door";

        tileBaseObj.AddComponent<Door>();
        Wall wallObj = tileBaseObj.GetComponent<Wall>();
        if (wallObj != null) Destroy( wallObj );
        Tile groundObj = tileBaseObj.GetComponent<Tile>();
        if (groundObj != null) Destroy( groundObj );
    }
}
