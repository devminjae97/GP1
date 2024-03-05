using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class MapGeneratorIssac : MonoBehaviour
{
    [SerializeField] private int roomDepth = 10;
    [SerializeField] private int roomSize = 30;
    [SerializeField] private GameObject gridRenderer;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject tile;
    private SpriteRenderer groundSprite;
    private Vector2Int mapSize;
    private int[] xdir = new int[] { 0, 0, 1, -1 };
    private int[] ydir = new int[] { 1, -1, 0, 0 };
    private Cell[,] cellList;
    private int roomCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        InitMap();
        DrawGrid();
        GenerateRoom( null, cellList[ cellList.GetLength( 1 ) / 2, cellList.GetLength( 0 ) / 2 ] );
    }

    void InitMap()
    {
        // mapSize: map의 크기
        // cellList: cell의 가로 개수 * cell의 세로 개수 (총 cell 개수)
        //roomSize = RoomTypeManager.GetInstance().SpriteSize * 2;
        mapSize = new Vector2Int( roomDepth * roomSize * 4, roomDepth * roomSize * 4 );
        cellList = new Cell[roomDepth * 4, roomDepth * 4];
        for (int i = 0; i < roomDepth * 4; i++)
        {
            for (int j = 0; j < roomDepth * 4; j++)
            {
                cellList[i, j] = new Cell( new Vector2Int( i, j ), new Vector2( -mapSize.x / 2 + i * roomSize + roomSize / 2, mapSize.y / 2 - j * roomSize + roomSize / 2 ) );
            }
        }
    }

    void DrawGrid()
    {
        LineRenderer grid = Instantiate( gridRenderer ).GetComponent<LineRenderer>();

        grid.SetPosition( 0, new Vector2( 0, 0 ) - mapSize / 2 );
        grid.SetPosition( 1, new Vector2( mapSize.x, 0 ) - mapSize / 2 );
        grid.SetPosition( 2, new Vector2( mapSize.x, mapSize.y ) - mapSize / 2 );
        grid.SetPosition( 3, new Vector2( 0, mapSize.y ) - mapSize / 2 );

        for (int i = roomSize; i < mapSize.x; i += roomSize)
        {
            int pos = -mapSize.x / 2 + i;
            DrawLine( new Vector2( pos, -mapSize.y / 2 ), new Vector2( pos, mapSize.y / 2 ) );
            DrawLine( new Vector2( -mapSize.y / 2, pos ), new Vector2( mapSize.y / 2, pos ) );
        }
    }

    void DrawLine( Vector2 from, Vector2 to )
    {
        LineRenderer line = Instantiate( lineRenderer ).GetComponent<LineRenderer>();

        line.SetPosition( 0, from);
        line.SetPosition( 1, to);
    }

    /*
     * pos: room's left down pos
     * isSpecialRoom: start or boss room
     */
    void GenerateRoom( Cell prevCell, Cell curCell )
    {
        if (roomCount > roomDepth)
            return;

        (bool, List<Cell>) checkRoomResult = CheckValidRoom( curCell );

        if (checkRoomResult.Item1 == false) 
            return;

        // test: 기준 방 확인용
        groundSprite = Instantiate( tile ).GetComponent<SpriteRenderer>();
        groundSprite.color = Color.yellow;
        groundSprite.transform.localScale = new Vector3( 1, 1, 0 );
        groundSprite.transform.position = new Vector3( curCell.posWorld.x, curCell.posWorld.y, 0 );

        foreach (Cell cellPos in checkRoomResult.Item2)
        {
            cellList[cellPos.pos.x, cellPos.pos.y].isChecked = true;
            cellList[cellPos.pos.x, cellPos.pos.y].id = roomCount;

            groundSprite = Instantiate( tile ).GetComponent<SpriteRenderer>();
            groundSprite.transform.position = new Vector3( cellPos.posWorld.x, cellPos.posWorld.y, 0 );
            groundSprite.transform.localScale = new Vector3( roomSize, roomSize, 0 );
            groundSprite.color = new Color( roomCount * (1 / (float)roomDepth), roomCount * (1 / (float)roomDepth), roomCount * (1 / (float)roomDepth) );
            groundSprite.sortingOrder = -3;

            for (int i = 0; i < 4; i++)
            {
                int nx = cellPos.pos.x + xdir[i];
                int ny = cellPos.pos.y + ydir[i];
                if (0 < nx && nx <= cellList.GetLength(1) && 0 < ny && ny <= cellList.GetLength( 0 ) && cellList[nx, ny].id != cellPos.id)
                {
                    if (cellPos.IsInjected( cellList[nx, ny].id ) || cellList[nx, ny].id == 0)
                        continue;

                    cellPos.injectedCellList.Add( cellList[nx, ny].id );
                    cellList[nx, ny].injectedCellList.Add( cellPos.id );
                    GenerateDoor( cellPos, cellList[nx, ny] );
                }
            }
        }
        roomCount++;

        // 주변 사용하지 않은 cell 탐색
        HashSet<(Cell, Cell)> nearCells = GetNearCells( checkRoomResult.Item2 );
        foreach ((Cell, Cell) cellPos in nearCells)
        {
            GenerateRoom( cellPos.Item1, cellPos.Item2 );
            if (roomCount > roomDepth)
            {
                return;
            }
        }
    }

    void GenerateDoor( Cell prevCell, Cell postCell )
    {
        int xgap = prevCell.pos.x - postCell.pos.x;
        int ygap = prevCell.pos.y - postCell.pos.y;
       
        if (xgap < 0 && ygap == 0)
        {
            prevCell.doors[1] = true;
            postCell.doors[3] = true;
        }
        else if (xgap > 0 && ygap == 0)
        {
            prevCell.doors[3] = true;
            postCell.doors[1] = true;
        }
        else if (xgap == 0 && ygap > 0)
        {
            prevCell.doors[0] = true;
            postCell.doors[2] = true;
        }
        else if (xgap == 0 && ygap < 0)
        {
            prevCell.doors[2] = true;
            postCell.doors[0] = true;
        }

        groundSprite = Instantiate( tile ).GetComponent<SpriteRenderer>();
        groundSprite.color = Color.red;
        groundSprite.transform.localScale = new Vector3( 1, 1, 0 );
        groundSprite.transform.position = (prevCell.posWorld + postCell.posWorld) / 2;
    }

    (bool, List<Cell>) CheckValidRoom( Cell cell )
    {
        bool canGenerate;
        int x, y;
        int roomTypeNum = RoomTypeManager.GetInstance().RoomTypes.Length;
        List<Cell> posList = new List<Cell>();
        // 블록의 크기를 정하기 위한 랜덤 배열
        int[] currentRoomSizeArray = new int[roomTypeNum];
        // 크기가 정해진 블록 중 어떤 모양을 선택할지에 대한 랜덤 배열
        int[] currentRoomTypeArray;
        // 블록 모양의 각 셀 좌표
        Vector2Int[] roomBlocks;

        for (int i = 0; i < roomTypeNum; i++)
            currentRoomSizeArray[i] = i;

        currentRoomSizeArray = ShuffleArray<int>(currentRoomSizeArray);

        foreach (int currentRoomSize in currentRoomSizeArray)
        {
            currentRoomTypeArray = new int[RoomTypeManager.GetInstance().RoomTypes[currentRoomSize].Length];

            for (int i = 0; i < currentRoomTypeArray.Length; i++)
                currentRoomTypeArray[i] = i;
            
            currentRoomTypeArray = ShuffleArray<int>( currentRoomTypeArray );
            foreach (int currentRoomType in currentRoomTypeArray)
            {
                // 블록 모양의 각 셀 좌표
                roomBlocks = RoomTypeManager.GetInstance().RoomTypes[currentRoomSize][currentRoomType];
                canGenerate = true;
                posList.Clear();
                foreach (Vector2Int blockPos in roomBlocks)
                {
                    x = cell.pos.x + blockPos.x;
                    y = cell.pos.y + blockPos.y;
                    // 맵을 벗어나거나 이미 생성된 셀이면 다른 자리를 찾아봐야 한다.
                    if (x < 0 || x >= cellList.GetLength( 1 ) || y < 0 || y >= cellList.GetLength( 0 ) || cellList[x, y].isChecked)
                    {
                        canGenerate = false;
                        break;
                    }
                    posList.Add( cellList[x, y] );
                }
                if (canGenerate)
                {
                    // test용으로 sprite붙여보자
                    return (true, posList);
                }
            }
        }
        return (false, posList);
    }

    // (기준, 인접)
    HashSet<(Cell, Cell)> GetNearCells( List<Cell> suburbCellList )
    {
        HashSet<(Cell, Cell)> nearCells = new HashSet<(Cell, Cell)>();
        suburbCellList = ShuffleList<Cell>( suburbCellList );
        foreach (Cell curPos in suburbCellList)
        {
            for (int i = 0; i < 4; i++)
            {
                int x = curPos.pos.x + xdir[i];
                int y = curPos.pos.y + ydir[i];
                if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y || cellList[x, y].isChecked) continue;
                nearCells.Add( (curPos, cellList[x, y]) );
            }
        }
        return nearCells;
    }

    private T[] ShuffleArray<T>( T[] array )
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < array.Length; ++i)
        {
            random1 = UnityEngine.Random.Range( 0, array.Length );
            random2 = UnityEngine.Random.Range( 0, array.Length );

            temp = array[random1];
            array[random1] = array[random2];
            array[random2] = temp;
        }

        return array;
    }

    private List<T> ShuffleList<T>( List<T> list )
    {
        int random1, random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = UnityEngine.Random.Range( 0, list.Count );
            random2 = UnityEngine.Random.Range( 0, list.Count );

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }
}
