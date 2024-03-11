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

   /* private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float inputX = Input.GetAxisRaw( "Horizontal" );
        float inputY = Input.GetAxisRaw( "Vertical" );

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
        }

        rigid.velocity = new Vector2( speed * inputX * inputY, rigid.velocity.y );
    }*/
}
