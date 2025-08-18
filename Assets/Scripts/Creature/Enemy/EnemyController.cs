using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 walkDirectionVector = Vector2.right;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    public enum WalkableDirection { Right, Left };

    private WalkableDirection _walkDirection;
    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set { 
            if(_walkDirection!=value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if(value==WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if(value==WalkableDirection.Left)
                    walkDirectionVector= Vector2.left;
            }

            _walkDirection = value; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            FlipDirection();
        rb.linearVelocity = new Vector2(moveSpeed * walkDirectionVector.x, rb.linearVelocity.y);
    }

    private void FlipDirection()
    {
        if(WalkDirection == WalkableDirection.Right)
            WalkDirection = WalkableDirection.Left;
        else if(WalkDirection == WalkableDirection.Left)
            WalkDirection = WalkableDirection.Right;
        else
        {
            Debug.LogError("Current Walkable direction is not set to legal valus of right or left");
        }
    }
}
