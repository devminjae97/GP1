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
}

public class RoomTypeManager : MonoBehaviour
{
    // 싱글톤 인스턴스
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
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(-1, 0) },              // 오른쪽 위
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 1 ), new Vector2Int(1, 0) },              // 오른쪽 아래
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, 1) },              // 아래 오른쪽
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(0, -1) },              // 아래 왼쪽
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(-1, 0) },              // 왼쪽 위
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, -1 ), new Vector2Int(1, 0) },              // 왼쪽 아래
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, 1) },              // 위 오른쪽
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(0, -1) },              // 위 왼쪽          
    };         
    public Vector2Int[][] size4 = new Vector2Int[][]
    {
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1, 1), new Vector2Int(0, 1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 1, 0 ), new Vector2Int(1,- 1), new Vector2Int(0, -1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, 1), new Vector2Int(0, 1) },
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( -1, 0 ), new Vector2Int(-1, -1), new Vector2Int(0, -1) }
    };

    public Vector2Int[][][] RoomTypes = new Vector2Int[4][][];
    [SerializeField] private GameObject tileObj;
    [SerializeField] private Sprite leftUp;
    [SerializeField] private Sprite leftDown;
    [SerializeField] private Sprite rightUp;
    [SerializeField] private Sprite rightDown;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    [SerializeField] private Sprite normal;
    [SerializeField] private int spriteSize = 10;
    [SerializeField] private Dictionary<ETileType, Sprite> spriteDic;
    SpriteRenderer spriteObj;

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }
        else
        {
            Destroy( gameObject );
        }

        // RoomTypes 배열 초기화
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
            { ETileType.eNormal, normal }};
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

    public void DrawTile( ETileType tileType, Vector2 pos )
    {
        spriteObj = Instantiate( tileObj ).GetComponent<SpriteRenderer>();
        spriteObj.sprite = spriteDic[tileType];
        spriteObj.transform.position = new Vector3( pos.x, pos.y, 0 );
        spriteObj.transform.localScale = new Vector2( spriteSize, spriteSize);
    }
}
