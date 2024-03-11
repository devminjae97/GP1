using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class MapGeneratorIssac : MonoBehaviour
{
    private float tileSizePerCell;
    private int roomCount = 1;
    // 오른쪽, 왼쪽, 아래, 위, 왼쪽아래, 오른쪽아래, 왼쪽위, 오른쪽위
    private int[] xdir = new int[] { 0, 0, 1, -1, 1, 1, -1, -1 };
    private int[] ydir = new int[] { 1, -1, 0, 0, -1, 1, -1, 1 };
    [SerializeField] private int roomDepth = 10;
    [SerializeField] private int cellSize = 30;
    [SerializeField, Tooltip("cellSize % tileNumPerCell == 0")] private int tileNumPerCell = 30;
    [SerializeField] private GameObject gridRenderer;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject tileBase;
    [SerializeField] private Cell[,] cellList;
    private SpriteRenderer groundSprite;
    private Vector2Int mapSize;

    // Start is called before the first frame update
    void Start()
    {
        InitMap();
        DrawGrid();
        GenerateRoom( cellList[ cellList.GetLength( 1 ) / 2, cellList.GetLength( 0 ) / 2 ] );
    }

    void InitMap()
    {
        int cellNum = roomDepth * 4;
        mapSize = new Vector2Int( cellNum * cellSize, cellNum * cellSize );
        cellList = new Cell[cellNum, cellNum];
        tileSizePerCell = cellSize / tileNumPerCell;
        for (int i = 0; i < cellNum; i++)
        {
            for (int j = 0; j < cellNum; j++)
            {
                cellList[i, j] = new Cell( new Vector2Int( i, j ), new Vector2( -mapSize.x / 2 + j * cellSize, mapSize.y / 2 - i * cellSize  ) );
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

        for (int i = cellSize; i < mapSize.x; i += cellSize)
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
    void GenerateRoom( Cell stdCell )
    {
        int nx, ny;

        if (roomCount > roomDepth)
            return;

        (bool, List<Cell>) checkRoomResult = CheckValidRoom( stdCell );

        if (checkRoomResult.Item1 == false) 
            return;

        DrawTestStdCell(stdCell);

        foreach (Cell cell in checkRoomResult.Item2)
        {
            InitTilesOfCell( cell );
            DrawTestCells( cell );

            cell.isChecked = true;
            cell.id = roomCount;

            for (int i = 0; i < 4; i++)
            {
                nx = cell.pos.x + xdir[i];
                ny = cell.pos.y + ydir[i];
                if (0 < nx && nx <= cellList.GetLength(1) && 0 < ny && ny <= cellList.GetLength( 0 ) && cellList[nx, ny].id != cell.id && cellList[nx, ny].id != 0)
                {
                    if (cell.IsInjected( cellList[nx, ny].id ))
                        continue;     

                    cell.injectedCellList.Add( cellList[nx, ny].id );
                    cellList[nx, ny].injectedCellList.Add( cell.id );

                    if (i == 0) GenerateDoor( cell, cellList[nx, ny], i );
                    else if (i == 1) GenerateDoor( cellList[nx, ny], cell, i );
                    else if (i == 2) GenerateDoor( cell, cellList[nx, ny], i );
                    else if (i == 3) GenerateDoor( cellList[nx, ny], cell, i );
                }
            }
        }
        roomCount++;
        SetTileMap(checkRoomResult.Item2);

        // 주변 사용하지 않은 cell 탐색
        HashSet<Cell> nearCells = GetNearCells( checkRoomResult.Item2 );
        foreach (Cell cell in nearCells)
        {
            GenerateRoom(cell );
            if (roomCount > roomDepth)
                return;
        }
    }

    void DrawTestStdCell(Cell cell)
    {
        groundSprite = Instantiate( tile ).GetComponent<SpriteRenderer>();
        groundSprite.color = Color.yellow;
        groundSprite.transform.localScale = new Vector3( 1, 1, 0 );
        groundSprite.transform.position = new Vector3( cell.posWorld.x, cell.posWorld.y, 0 );
    }

    void DrawTestCells(Cell cell)
    {
        groundSprite = Instantiate( tile ).GetComponent<SpriteRenderer>();
        groundSprite.transform.position = cell.posWorld;
        groundSprite.transform.localScale = new Vector3( cellSize, cellSize, 0 );
        groundSprite.color = new Color( roomCount * (1 / (float)roomDepth), roomCount * (1 / (float)roomDepth), roomCount * (1 / (float)roomDepth) );
        groundSprite.sortingOrder = -3;
    }

    void InitTilesOfCell(Cell cell)
    {
        cell.tiles = new GameObject[tileNumPerCell, tileNumPerCell];
        for (int i = 0; i < tileNumPerCell; i++)
        {
            for (int j = 0; j < tileNumPerCell; j++)
            {
                cell.tiles[i, j] = Instantiate( tileBase );
                cell.tiles[i, j].GetComponent<TileBase>().posWorld = new Vector2( cell.posWorld.x - cellSize / 2 + j * tileSizePerCell, cell.posWorld.y + cellSize / 2 - i * tileSizePerCell );
                cell.tiles[i, j].GetComponent<TileBase>().Scale = tileSizePerCell;
            }
        }
    }

    void SetTileMap( List<Cell> cells )
    {
        bool rOk, lOk, uOk, dOk, ldOk, rdOk, luOk, ruOk;
        int rx, ry, lx, ly, ux, uy, dx, dy, ldx, ldy, rdx, rdy, lux, luy, rux, ruy;
        foreach (Cell cell in cells)
        { 
            rx = cell.pos.x + xdir[0];
            ry = cell.pos.y + ydir[0];
            lx = cell.pos.x + xdir[1];
            ly = cell.pos.y + ydir[1];
            dx = cell.pos.x + xdir[2];
            dy = cell.pos.y + ydir[2];
            ux = cell.pos.x + xdir[3];
            uy = cell.pos.y + ydir[3];
            ldx = cell.pos.x + xdir[4];
            ldy = cell.pos.y + ydir[4];
            rdx = cell.pos.x + xdir[5];
            rdy = cell.pos.y + ydir[5];
            lux = cell.pos.x + xdir[6];
            luy = cell.pos.y + ydir[6];
            rux = cell.pos.x + xdir[7];
            ruy = cell.pos.y + ydir[7];

            rOk = 0 <= rx && rx < cellList.GetLength( 1 ) && 0 <= ry && ry < cellList.GetLength( 0 ) && cellList[rx, ry].id == cell.id;
            lOk = 0 <= lx && lx < cellList.GetLength( 1 ) && 0 <= ly && ly < cellList.GetLength( 0 ) && cellList[lx, ly].id == cell.id;
            uOk = 0 <= ux && ux < cellList.GetLength( 1 ) && 0 <= uy && uy < cellList.GetLength( 0 ) && cellList[ux, uy].id == cell.id;
            dOk = 0 <= dx && dx < cellList.GetLength( 1 ) && 0 <= dy && dy < cellList.GetLength( 0 ) && cellList[dx, dy].id == cell.id;
            ldOk = 0 <= ldx && ldx < cellList.GetLength( 1 ) && 0 <= ldy && ldy < cellList.GetLength( 0 ) && cellList[ldx, ldy].id == cell.id;
            rdOk = 0 <= rdx && rdx < cellList.GetLength( 1 ) && 0 <= rdy && rdy < cellList.GetLength( 0 ) && cellList[rdx, rdy].id == cell.id;
            luOk = 0 <= lux && lux < cellList.GetLength( 1 ) && 0 <= luy && luy < cellList.GetLength( 0 ) && cellList[lux, luy].id == cell.id;
            ruOk = 0 <= rux && rux < cellList.GetLength( 1 ) && 0 <= ruy && ruy < cellList.GetLength( 0 ) && cellList[rux, ruy].id == cell.id;

            // right up
            if (rOk && uOk)
            {
                if (ruOk) DrawGround( ETileType.eNormal, cell.tiles[0, tileNumPerCell - 1], tileSizePerCell );
                else DrawWall( ETileType.eUp, cell.tiles[0, tileNumPerCell - 1], tileSizePerCell );
            }
            else if (rOk && !uOk) DrawWall( ETileType.eUp, cell.tiles[0, tileNumPerCell - 1], tileSizePerCell );
            else if (!rOk && uOk) DrawWall( ETileType.eRight, cell.tiles[0, tileNumPerCell - 1], tileSizePerCell );
            else if (!rOk && !uOk) DrawWall( ETileType.eRightUp, cell.tiles[0, tileNumPerCell - 1], tileSizePerCell );

            // right down
            if (rOk && dOk)
            {
                if (rdOk) DrawGround( ETileType.eNormal, cell.tiles[tileNumPerCell - 1, tileNumPerCell - 1], tileSizePerCell );
                else DrawWall( ETileType.eUp, cell.tiles[tileNumPerCell - 1, tileNumPerCell - 1], tileSizePerCell );
            }
            else if (rOk && !dOk) DrawWall( ETileType.eDown, cell.tiles[tileNumPerCell - 1, tileNumPerCell - 1], tileSizePerCell );
            else if (!rOk && dOk) DrawWall( ETileType.eRight, cell.tiles[tileNumPerCell - 1, tileNumPerCell - 1], tileSizePerCell );
            else if (!rOk && !dOk) DrawWall( ETileType.eRightDown, cell.tiles[tileNumPerCell - 1, tileNumPerCell - 1], tileSizePerCell );

            // left up
            if (lOk && uOk)
            {
                if (luOk) DrawGround( ETileType.eNormal, cell.tiles[0, 0], tileSizePerCell );
                else DrawWall( ETileType.eUp, cell.tiles[0, 0], tileSizePerCell );
            }
            else if (lOk && !uOk) DrawWall( ETileType.eUp, cell.tiles[0, 0], tileSizePerCell );
            else if (!lOk && uOk) DrawWall( ETileType.eLeft, cell.tiles[0, 0], tileSizePerCell );
            else if (!lOk && !uOk) DrawWall( ETileType.eLeftUp, cell.tiles[0, 0], tileSizePerCell );

            // left down
            if (lOk && dOk)
            {
                if (ldOk) DrawGround( ETileType.eNormal, cell.tiles[tileNumPerCell - 1, 0], tileSizePerCell );
                else DrawWall( ETileType.eUp, cell.tiles[tileNumPerCell - 1, 0], tileSizePerCell );
            }
            else if (lOk && !dOk) DrawWall( ETileType.eDown, cell.tiles[tileNumPerCell - 1, 0], tileSizePerCell );
            else if (!lOk && dOk) DrawWall( ETileType.eLeft, cell.tiles[tileNumPerCell - 1, 0], tileSizePerCell );
            else if (!lOk && !dOk) DrawWall( ETileType.eLeftDown, cell.tiles[tileNumPerCell - 1, 0], tileSizePerCell );
           
            for (int i = 1; i < tileNumPerCell - 1; i++)
            {
                // down
                if (!dOk) DrawWall( ETileType.eDown, cell.tiles[tileNumPerCell - 1, i], tileSizePerCell );
                else DrawGround( ETileType.eNormal, cell.tiles[tileNumPerCell - 1, i], tileSizePerCell );
                // up
                if (!uOk) DrawWall( ETileType.eUp, cell.tiles[0, i], tileSizePerCell );
                else DrawGround( ETileType.eNormal, cell.tiles[0, i], tileSizePerCell );
                // left
                if (!lOk) DrawWall( ETileType.eLeft, cell.tiles[i, 0], tileSizePerCell );
                else DrawGround( ETileType.eNormal, cell.tiles[i, 0], tileSizePerCell );
                // right
                if (!rOk) DrawWall( ETileType.eRight, cell.tiles[i, tileNumPerCell - 1], tileSizePerCell );
                else DrawGround( ETileType.eNormal, cell.tiles[i, tileNumPerCell - 1], tileSizePerCell );
            }

            // inner
            for (int i = 1; i < tileNumPerCell - 1; i++)
            {
                for (int j = 1; j < tileNumPerCell - 1; j++)
                {
                    DrawGround( ETileType.eNormal, cell.tiles[i, j], tileSizePerCell );
                }
            }
        }
    }

    void DrawGround( ETileType tileType, GameObject pos, float scaleValue )
    {
        RoomTypeManager.GetInstance().DrawGround( tileType, pos, scaleValue );
    }

    void DrawWall( ETileType tileType, GameObject pos, float scaleValue )
    {
        RoomTypeManager.GetInstance().DrawWall( tileType, pos, scaleValue );
    }

    void DrawDoor( ETileType tileType, GameObject tileBaseObj, float scaleValue, Color color )
    {
        RoomTypeManager.GetInstance().DrawDoor( tileType, tileBaseObj, scaleValue, color );
    }

    void GenerateDoor( Cell prevCell, Cell postCell, int dir )
    {
        // 문 방향, 위치 정책
        if (dir == 0 || dir == 1)
        {
            DrawDoor( ETileType.eDoor, prevCell.tiles[tileNumPerCell / 2, tileNumPerCell - 1], tileSizePerCell, Color.red );
            DrawDoor( ETileType.eDoor, postCell.tiles[tileNumPerCell / 2, 0], tileSizePerCell, Color.blue );
            SetNextDoor( prevCell.tiles[tileNumPerCell / 2, tileNumPerCell - 1], postCell.tiles[tileNumPerCell / 2, 0] );
        }
        else
        {
            DrawDoor( ETileType.eDoor, prevCell.tiles[tileNumPerCell - 1, tileNumPerCell / 2], tileSizePerCell, Color.green );
            DrawDoor( ETileType.eDoor, postCell.tiles[0, tileNumPerCell / 2], tileSizePerCell, Color.yellow );
            SetNextDoor( prevCell.tiles[tileNumPerCell - 1, tileNumPerCell / 2], postCell.tiles[0, tileNumPerCell / 2] );
        }
    }

    void SetNextDoor(GameObject prevTile, GameObject postTile)
    {
        Door prevDoor = prevTile.GetComponent<Door>();
        Door postDoor = postTile.GetComponent<Door>();
        if (prevDoor && postDoor)
        {
            prevDoor.NextDoor = postDoor;
            postDoor.NextDoor = prevDoor;
            prevDoor.NextDoorPos = postDoor.GetComponent<TileBase>().posWorld;
            postDoor.NextDoorPos = prevDoor.GetComponent<TileBase>().posWorld;
        }
    }

    (bool, List<Cell>) CheckValidRoom( Cell cell )
    {
        bool canGenerate;
        int nx, ny;
        int roomTypeNum = RoomTypeManager.GetInstance().RoomTypes.Length;
        List<Cell> posList = new List<Cell>();
        // 블록의 크기를 정하기 위한 랜덤 배열
        int[] currentcellSizeArray = new int[roomTypeNum];
        // 크기가 정해진 블록 중 어떤 모양을 선택할지에 대한 랜덤 배열
        int[] currentRoomTypeArray;
        // 블록 모양의 각 셀 좌표
        Vector2Int[] roomBlocks;

        for (int i = 0; i < roomTypeNum; i++)
            currentcellSizeArray[i] = i;

        currentcellSizeArray = ShuffleArray<int>(currentcellSizeArray);

        foreach (int currentcellSize in currentcellSizeArray)
        {
            currentRoomTypeArray = new int[RoomTypeManager.GetInstance().RoomTypes[currentcellSize].Length];

            for (int i = 0; i < currentRoomTypeArray.Length; i++)
                currentRoomTypeArray[i] = i;
            
            currentRoomTypeArray = ShuffleArray<int>( currentRoomTypeArray );
            foreach (int currentRoomType in currentRoomTypeArray)
            {
                // 블록 모양의 각 셀 좌표
                roomBlocks = RoomTypeManager.GetInstance().RoomTypes[currentcellSize][currentRoomType];
                canGenerate = true;
                posList.Clear();
                foreach (Vector2Int blockPos in roomBlocks)
                {
                    nx = cell.pos.x + blockPos.x;
                    ny = cell.pos.y + blockPos.y;
                    // 맵을 벗어나거나 이미 생성된 셀이면 다른 자리를 찾아봐야 한다.
                    if (nx < 0 || nx >= cellList.GetLength( 1 ) || ny < 0 || ny >= cellList.GetLength( 0 ) || cellList[nx, ny].isChecked)
                    {
                        canGenerate = false;
                        break;
                    }
                    posList.Add( cellList[nx, ny] );
                }
                if (canGenerate)
                {
                    return (true, posList);
                }
            }
        }
        return (false, posList);
    }

    // (기준, 인접)
    HashSet<Cell> GetNearCells( List<Cell> suburbCellList )
    {
        int nx, ny;
        HashSet<Cell> nearCells = new HashSet<Cell>();
        suburbCellList = ShuffleList<Cell>( suburbCellList );
        foreach (Cell curPos in suburbCellList)
        {
            for (int i = 0; i < 4; i++)
            {
                nx = curPos.pos.x + xdir[i];
                ny = curPos.pos.y + ydir[i];
                if (nx < 0 || nx >= mapSize.x || ny < 0 || ny >= mapSize.y || cellList[nx, ny].isChecked) continue;
                nearCells.Add( cellList[nx, ny] );
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
