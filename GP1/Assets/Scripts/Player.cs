using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movePower = 1f;
    public float jumpPower = 1f;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;

    Vector3 movement;
    bool isJumping = false;
    public bool isGrounded = true;

    private int jumpCount = 1;

    [SerializeField]
    private GameObject[] groundCheckers = new GameObject[3];
    [SerializeField]
    private float groundCheckerDistance = 3f;
    RaycastHit2D groundCheckerHit2D;
    LayerMask GroundMask;
    public float angle;
    public Vector2 perp;
    public bool isSlope;
    public Transform frontObject;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = rigid.GetComponentInChildren<SpriteRenderer>();
        GroundMask = LayerMask.GetMask( "Ground" );
    }

    void Update()
    {
        GroundCheck();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isJumping = true;
        }

        // 경사에서 움직이지 않도록
        if (Input.GetAxis("Horizontal") == 0)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 경사 움직이기
        RaycastHit2D hit = Physics2D.Raycast( transform.position, Vector2.down, 3, GroundMask );
        SlopeCheck( hit );

        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
        Debug.DrawLine( hit.point, hit.point + perp, Color.red );

        RaycastHit2D fronthit = Physics2D.Raycast( frontObject.position, transform.right, 1, GroundMask );
        if (hit||fronthit)
        {
            if (fronthit)
            {
                SlopeCheck( fronthit );
            }
            else if (hit)
            {
                SlopeCheck( hit );
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float inputX = Input.GetAxisRaw( "Horizontal" );
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            spriteRenderer.flipX = true;
            frontObject.transform.localPosition = new Vector3( -0.5f, 0, 0 );
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            spriteRenderer.flipX = false;
            frontObject.transform.localPosition = new Vector3( 0.5f, 0, 0 );
        }

        if (isSlope && isGrounded && !isJumping && angle < 45)
        {
            rigid.velocity = perp * movePower * inputX * -1f;
        }
        else if (!isSlope && isGrounded && !isJumping)
        {
            rigid.velocity = new Vector2( movePower * inputX, 0);
        }
        else if (!isGrounded)
        {
            rigid.velocity = new Vector2( movePower * inputX, rigid.velocity.y );
        }
    }

    private void Jump()
    {
        if (!isJumping)
            return;

        rigid.velocity = Vector2.zero;
        Vector2 jumpVelocity = new Vector2( 0, jumpPower );
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumping = false;
    }

    private void GroundCheck()
    {
        foreach (GameObject groundChecker in groundCheckers)
        {
            Debug.DrawRay( groundChecker.transform.position, groundChecker.transform.up * groundCheckerDistance, Color.red );
            groundCheckerHit2D = Physics2D.Raycast( groundChecker.transform.position, -groundChecker.transform.up, groundCheckerDistance, LayerMask.GetMask( "Ground" ) );
            if (groundCheckerHit2D)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    private void SlopeCheck( RaycastHit2D hit )
    {
        perp = Vector2.Perpendicular( hit.normal ).normalized;
        angle = Vector2.Angle( hit.normal, Vector2.up );

        if (angle != 0)
        {
            isSlope = true;
        }
        else
        {
            isSlope = false;
        }
    }
}
