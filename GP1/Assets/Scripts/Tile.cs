using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : TileBase
{
    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
        }
    }

    public Tile( Vector2 _posWorld ) : base( _posWorld )
    {
        this.posWorld = _posWorld;
    }
}
