using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        new Vector2Int[] { new Vector2Int( 0, 0 ), new Vector2Int( 0, 0 ) },
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
    [SerializeField] private GameObject tile;
    [SerializeField] private SpriteRenderer leftUp;
    [SerializeField] private SpriteRenderer leftDown;
    [SerializeField] private SpriteRenderer rightUp;
    [SerializeField] private SpriteRenderer rightDown;
    [SerializeField] private SpriteRenderer left;
    [SerializeField] private SpriteRenderer right;
    [SerializeField] private SpriteRenderer up;
    [SerializeField] private SpriteRenderer down;
    private int spriteSize = 5;
    
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

        if (leftUp != null)
            spriteSize = (int)leftUp.size.x;
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

    public void SetTileMap(int blockSize, int blockType, Cell stdCell)
    {
        /*SpriteRenderer spriteObj;
        if (blockSize == 1)
        {
            spriteObj = Instantiate( tile ).GetComponent<SpriteRenderer>();
            sprite stdCell.posWorld.x
        }*/
    }
}
