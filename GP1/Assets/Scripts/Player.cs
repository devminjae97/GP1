using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private PlayerController controller;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;


    void Start()
    {
        controller = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
        Vector2 moveVelocity = moveInput.normalized * speed;
        controller.Move( moveVelocity );
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
}
