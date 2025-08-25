using System;
using Assets.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float walkStopRate = 0.6f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    private Vector2 walkDirectionVector = Vector2.right;
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

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        set
        {
            _hasTarget = value;
            animator?.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }

    Rigidbody2D rb;
    Animator animator;
    Status status;
    TouchingDirections touchingDirections;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        status = GetComponent<Status>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void Update()
    {
        HasTarget = attackZone?.detectedColliders.Count > 0;

        if(AttackCooldown > 0)
            AttackCooldown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            FlipDirection();

        if(!status.LockVelocity)
        {
            if (CanMove)
                rb.linearVelocity = new Vector2(moveSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocityX, 0, walkStopRate), rb.linearVelocity.y);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocityY + knockback.y);
    }

    public void OnCliffDetected()
    {
        if(touchingDirections.IsGrounded)
            FlipDirection();
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
