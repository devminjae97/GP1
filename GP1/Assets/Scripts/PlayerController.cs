using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 velocity;
    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rigid.MovePosition( rigid.position + velocity * Time.fixedDeltaTime );
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
}
