using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using TreeEditor;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.XR;

public class MapGenerator : MonoBehaviour
{
    MapNode rootNode;
    [SerializeField] Vector2Int mapSize;
    [SerializeField] int minNodeSize;
    [SerializeField] private GameObject gridRenderer;
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private GameObject roomRenderer;
    [SerializeField] private GameObject passageRenderer;
    [SerializeField] int treeDepth;
    [SerializeField, Range( 0.01f, 0.5f ), Tooltip( "0.01f ~ 0.5f" )] private float roomMinPadding;
    [SerializeField, Range( 0.5f, 0.99f ), Tooltip( "0.5f ~ 0.99f" )] private float roomMaxPadding;

    private void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        InitializeMapTree();
        DevideNode( rootNode, 0 );
        GenerateRoad( rootNode, 0 );
    }

    void InitializeMapTree()
    {
        rootNode = new MapNode( new RectInt( 0, 0, mapSize.x, mapSize.y ) );
        DrawGrid( rootNode.rect.x, rootNode.rect.y );
    }

    void DrawGrid( int x, int y )
    {
        LineRenderer grid = Instantiate( gridRenderer ).GetComponent<LineRenderer>();
        grid.SetPosition( 0, new Vector2( x, y ) - mapSize / 2 );
        grid.SetPosition( 1, new Vector2( x + mapSize.x, y ) - mapSize / 2 );
        grid.SetPosition( 2, new Vector2( x + mapSize.x, y + mapSize.y ) - mapSize / 2 );
        grid.SetPosition( 3, new Vector2( x, y + mapSize.y ) - mapSize / 2 );
    }

    void DrawLine( Vector2 from, Vector2 to )
    {
        LineRenderer line = Instantiate( lineRenderer ).GetComponent<LineRenderer>();
        line.SetPosition( 0, from - mapSize / 2 );
        line.SetPosition( 1, to - mapSize / 2 );
    }

    void DrawRoom( float x, float y, float width, float height, MapNode curNode )
    {
        LineRenderer grid = Instantiate( roomRenderer ).GetComponent<LineRenderer>();
        grid.SetPosition( 0, new Vector2( x, y ) - mapSize / 2 );
        grid.SetPosition( 1, new Vector2( x + width, y ) - mapSize / 2 );
        grid.SetPosition( 2, new Vector2( x + width, y + height ) - mapSize / 2 );
        grid.SetPosition( 3, new Vector2( x, y + height ) - mapSize / 2 );
    }

    void DrawPassage( Vector2 from, Vector2 to )
    {
        LineRenderer line = Instantiate( passageRenderer ).GetComponent<LineRenderer>();
        line.SetPosition( 0, from - mapSize / 2 );
        line.SetPosition( 1, to - mapSize / 2 );
    }

    void DevideNode( MapNode curNode, int depth )
    {
        if (depth >= treeDepth)
        {
            GenerateRoom( curNode );
            return;
        }

        int maxValue, randomPos;
        if (curNode.rect.width > curNode.rect.height)
        {
            maxValue = curNode.rect.width - minNodeSize;
            if (maxValue < minNodeSize)
            {
                GenerateRoom( curNode );
                return;
            }
            randomPos = Random.Range( minNodeSize, maxValue );
            curNode.left = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y, randomPos, curNode.rect.height ) );
            curNode.right = new MapNode( new RectInt( curNode.rect.x + randomPos, curNode.rect.y, curNode.rect.width - randomPos, curNode.rect.height ) );
            DrawLine( new Vector2( curNode.rect.x + randomPos, curNode.rect.y ), new Vector2( curNode.rect.x + randomPos, curNode.rect.y + curNode.rect.height ) );
        }
        else
        {
            maxValue = curNode.rect.height - minNodeSize;
            if (maxValue < minNodeSize)
            {
                GenerateRoom( curNode );
                return;
            }
            randomPos = Random.Range( minNodeSize, maxValue );
            curNode.left = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y, curNode.rect.width, randomPos ) );
            curNode.right = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y + randomPos, curNode.rect.width, curNode.rect.height - randomPos ) );
            DrawLine( new Vector2( curNode.rect.x, curNode.rect.y + randomPos ), new Vector2( curNode.rect.x + curNode.rect.width, curNode.rect.y + randomPos ) );
        }
        curNode.left.parent = curNode;
        curNode.right.parent = curNode;

        DevideNode( curNode.left, depth + 1 );
        DevideNode( curNode.right, depth + 1 );

        //DrawPassage( new Vector2( curNode.right.rect.x + curNode.right.rect.width / 2, curNode.right.rect.y + curNode.right.rect.height / 2 ), new Vector2( curNode.left.rect.x + curNode.left.rect.width / 2, curNode.left.rect.y + curNode.left.rect.height / 2 ) );
    }

    void GenerateRoom( MapNode curNode )
    {
        float width = Random.Range( curNode.rect.width * roomMinPadding, curNode.rect.width * roomMaxPadding );
        float height = Random.Range( curNode.rect.height * roomMinPadding, curNode.rect.height * roomMaxPadding );
        float widthPos = Random.Range( curNode.rect.x + 1, curNode.rect.x + curNode.rect.width - width - 1 );
        float heightPos = Random.Range( curNode.rect.y + 1, curNode.rect.y + curNode.rect.height - height - 1 );
        curNode.roomRect = new RectInt( Mathf.RoundToInt( widthPos ), Mathf.RoundToInt( heightPos ), Mathf.RoundToInt( width ), Mathf.RoundToInt( height ) );
        DrawRoom( widthPos, heightPos, width, height, curNode );
    }

    private void GenerateRoad( MapNode treeNode, int n ) //길 연결
    {
        if (n == treeDepth) return; //노드가 최하위일 때는 길을 연결하지 않음, 최하위 노드는 자식 트리가 없기 때문
        /*int x1 = GetCenterX( treeNode.left.roomRect ); //자식 트리의 던전 중앙 위치를 가져옴
        int x2 = GetCenterX( treeNode.right.roomRect );
        int y1 = GetCenterY( treeNode.left.roomRect );
        int y2 = GetCenterY( treeNode.right.roomRect );*/

        /*DrawPassage( new Vector2( x1, y1 ), new Vector2( x2, y2 ) ); //mapSize.x / 2를 빼는 이유는 화면 중앙에 맞추기 위함

        GenerateRoad( treeNode.left, n + 1 );
        GenerateRoad( treeNode.right, n + 1 );*/
    }

    private int GetCenterX( RectInt size )
    {
        return size.x + size.width / 2;
    }

    private int GetCenterY( RectInt size )
    {
        return size.y + size.height / 2;
    }
}
