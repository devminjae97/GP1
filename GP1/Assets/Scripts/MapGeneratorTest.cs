using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorTest : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] int widthNum;
    [SerializeField] int heightNum;
    [SerializeField] private GameObject gridRenderer;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject roomRenderer;
    [SerializeField] private GameObject passageRenderer;
    [SerializeField, Range( 0.01f, 0.5f ), Tooltip( "0.01f ~ 0.5f" )] private float roomMinPadding;
    [SerializeField, Range( 0.5f, 0.99f ), Tooltip( "0.5f ~ 0.99f" )] private float roomMaxPadding;
    MapNodeTest[,] NodeArray;

    private void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        NodeArray = new MapNodeTest[widthNum, heightNum];
        DrawMap();
    }

    void DrawMap()
    { 
        DrawGrids();
        DrawRooms();
        DrawPassages();
    }

    void DrawGrids()
    {
        LineRenderer grid = Instantiate( gridRenderer ).GetComponent<LineRenderer>();
        grid.SetPosition( 0, new Vector2( 0, 0 ) - mapSize / 2 );
        grid.SetPosition( 1, new Vector2( mapSize.x, 0 ) - mapSize / 2 );
        grid.SetPosition( 2, new Vector2(mapSize.x, mapSize.y ) - mapSize / 2 );
        grid.SetPosition( 3, new Vector2( 0, mapSize.y ) - mapSize / 2 );

        for (int i = 1; i <= widthNum - 1; i++)
        {
            int pos = mapSize.x / widthNum * i;
            DrawLine( new Vector2( pos, 0 ), new Vector2( pos, mapSize.y ) );
        }
        for (int i = 1; i <= heightNum - 1; i++)
        {
            int pos = mapSize.y / heightNum * i;
            DrawLine( new Vector2( 0, pos ), new Vector2( mapSize.x, pos ) );
        }
    }

    void DrawLine( Vector2 from, Vector2 to )
    {
        LineRenderer line = Instantiate( lineRenderer ).GetComponent<LineRenderer>();
        line.SetPosition( 0, from - mapSize / 2 );
        line.SetPosition( 1, to - mapSize / 2 );
    }

    void DrawRooms()
    {
        int nodeWidth = mapSize.x / widthNum;
        int nodeHeight = mapSize.y / heightNum;
        for (int i = 0; i < widthNum; i++)
        {
            for (int j = 0; j < heightNum; j++)
            {
                NodeArray[i, j] = new MapNodeTest( new RectInt( nodeWidth * i, nodeHeight * j, nodeWidth, nodeHeight ) );
                generateRoom( NodeArray[i, j] );
            }
        }
    }

    void generateRoom( MapNodeTest curNode )
    {
        float width = Random.Range( curNode.rect.width * roomMinPadding, curNode.rect.width * roomMaxPadding );
        float height = Random.Range( curNode.rect.height * roomMinPadding, curNode.rect.height * roomMaxPadding );
        float widthPos = Random.Range( curNode.rect.x + 1, curNode.rect.x + curNode.rect.width - width - 1 );
        float heightPos = Random.Range( curNode.rect.y + 1, curNode.rect.y + curNode.rect.height - height - 1 );
        DrawRoom( widthPos, heightPos, width, height, curNode );
    }


    void DrawRoom( float x, float y, float width, float height, MapNodeTest curNode )
    {
        LineRenderer grid = Instantiate( roomRenderer ).GetComponent<LineRenderer>();
        grid.SetPosition( 0, new Vector2( x, y ) - mapSize / 2 );
        grid.SetPosition( 1, new Vector2( x + width, y ) - mapSize / 2 );
        grid.SetPosition( 2, new Vector2( x + width, y + height ) - mapSize / 2 );
        grid.SetPosition( 3, new Vector2( x, y + height ) - mapSize / 2 );
    }

    void DrawPassages()
    {
        GeneratePassages();
    }

    void GeneratePassages()
    {
        int startNode = Random.Range(0, widthNum * heightNum);
        int r = startNode / widthNum;
        int c = startNode % widthNum;
        Queue<MapNodeTest> nodeQueue = new Queue<MapNodeTest>();
        nodeQueue.Enqueue( NodeArray[r, c] );
        NodeArray[r, c].isLinked = true;
    }
}
