using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public class Door : TileBase
{
    private bool isFirstCollide;
    [SerializeField] private Cell ownerCell;
    [SerializeField] private Door nextDoor;
    [SerializeField] private Vector2 nextDoorPos;
    [SerializeField] private SpriteRenderer minimapRenderer;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = true;
            boxCollider2D.isTrigger = true;
        }
    }

    public Door( Vector2 _posWorld ) : base( _posWorld )
    {
        this.posWorld = _posWorld;
    }

    private void OnTriggerEnter2D( Collider2D other )
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player)
        {
            if (isFirstCollide)
                return;

            EnterRoom( player );
        }
    }

    private void OnTriggerExit2D( Collider2D other )
    {
        isFirstCollide = false;
    }

    void EnterRoom( Player player )
    {
        nextDoor.IsFirstCollide = true;
        player.transform.position = nextDoorPos;

        ownerCell.EnterCell( nextDoor.OwnerCell );
    }

    public Cell OwnerCell
    {
        get { return ownerCell; }
        set { ownerCell = value; }
    }

    public Door NextDoor
    {
        get { return nextDoor; }
        set { nextDoor = value; }
    }

    public Vector2 NextDoorPos
    {
        get { return nextDoorPos; }
        set { nextDoorPos = value; }
    }

    public bool IsFirstCollide
    {
        get { return isFirstCollide; }
        set { isFirstCollide = value; }
    }
}
