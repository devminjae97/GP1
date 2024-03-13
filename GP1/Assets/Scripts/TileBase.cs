using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBase : MonoBehaviour
{
    public float scale;
    public Vector2 posWorld;
    private SpriteRenderer spriteRenderer;
    protected BoxCollider2D boxCollider2D;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public Vector2 PosWorld
    {
        get { return posWorld; }
        set { posWorld = value; }
    }

    public float Scale
    {
        get { return scale; }
        set 
        { 
            scale = value;
            boxCollider2D.size = new Vector2( 1, 1 );
        }
    }

    public TileBase(Vector2 _posWorld)
    {
        this.posWorld = _posWorld;
    }
}
