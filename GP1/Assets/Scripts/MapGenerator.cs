using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
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

        bool isLeaf = false;
        DevideNode(rootNode, 0, ref isLeaf);
    }

    void InitializeMapTree()
    {
        rootNode = new MapNode(new RectInt(0, 0, mapSize.x, mapSize.y ) );
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

    void DrawLine(Vector2 from, Vector2 to)
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

    void DevideNode(MapNode curNode, int depth, ref bool isLeafNode)
    {
        if (depth >= treeDepth)
        {
            generateRoom( curNode );
            isLeafNode = true;
            return;
        }

        int maxValue, randomPos;
        if (curNode.rect.width > curNode.rect.height)
        {
            maxValue = curNode.rect.width - minNodeSize;
            if (maxValue < minNodeSize)
            {
                generateRoom( curNode );
                isLeafNode = true;
                return;
            }
            randomPos = Random.Range( minNodeSize, maxValue );
            curNode.left = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y, randomPos, curNode.rect.height ) );
            curNode.right = new MapNode( new RectInt( curNode.rect.x + randomPos, curNode.rect.y, curNode.rect.width - randomPos, curNode.rect.height ) );
            DrawLine( new Vector2( curNode.rect.x + randomPos, curNode.rect.y ), new Vector2( curNode.rect.x + randomPos, curNode.rect.y + curNode.rect.height ));
        }
        else
        {
            maxValue = curNode.rect.height - minNodeSize;
            if (maxValue < minNodeSize)
            {
                generateRoom( curNode );
                isLeafNode = true;
                return;
            }
            randomPos = Random.Range( minNodeSize, maxValue );
            curNode.left = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y, curNode.rect.width, randomPos ) );
            curNode.right = new MapNode( new RectInt( curNode.rect.x, curNode.rect.y + randomPos, curNode.rect.width, curNode.rect.height - randomPos ) );
            DrawLine( new Vector2( curNode.rect.x, curNode.rect.y + randomPos ), new Vector2( curNode.rect.x + curNode.rect.width, curNode.rect.y + randomPos ));
        }
        curNode.left.parent = curNode;
        curNode.right.parent = curNode;

        bool isLeaf = false;
        DevideNode( curNode.left, depth + 1, ref isLeaf );
        DevideNode( curNode.right, depth + 1, ref isLeaf );

        if (isLeaf)
        {
            DrawPassage(new Vector2 (curNode.right.rect.x + curNode.right.rect.width / 2, curNode.right.rect.y + curNode.right.rect.height / 2 ), new Vector2(curNode.left.rect.x + curNode.left.rect.width / 2, curNode.left.rect.y + curNode.left.rect.height / 2 ) );
        }
    }

    void generateRoom(MapNode curNode)
    {
        float width = Random.Range( curNode.rect.width * roomMinPadding, curNode.rect.width * roomMaxPadding );
        float height = Random.Range( curNode.rect.height * roomMinPadding, curNode.rect.height * roomMaxPadding );
        float widthPos = Random.Range( curNode.rect.x + 1, curNode.rect.x + curNode.rect.width - width - 1 );
        float heightPos = Random.Range( curNode.rect.y + 1, curNode.rect.y + curNode.rect.height - height - 1 );
        DrawRoom( widthPos, heightPos, width, height, curNode );
    }
}
