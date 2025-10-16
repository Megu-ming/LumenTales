using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class EnemyController : CreatureController
{
    protected EnemyStatus status;
    protected ItemDropHelper itemDropHelper;

    [SerializeField]
    private EnemyData enemyData;

    float moveSpeed;
    float walkStopRate;
    [SerializeField] private DetectionZone attackZone;
    [SerializeField] private DetectionZone cliffDetectionZone;
    private Vector2 walkDirectionVector = Vector2.right;
    
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

    public bool CanMove { get { return animator.GetBool(AnimationStrings.canMove); } }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }

    TouchingDirections touchingDirections;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        status = GetComponent<EnemyStatus>();
        touchingDirections = GetComponent<TouchingDirections>();
        itemDropHelper = GetComponent<ItemDropHelper>();

        Init();
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

    private void Init()
    {
        if (enemyData != null)
        {
            gameObject.name = enemyData.enemyName;
            moveSpeed = enemyData.moveSpeed;
            walkStopRate = enemyData.walkStopRate;
            status.Type = enemyData.enemyType;
            status.BaseMaxHealth = enemyData.maxHp;
            status.CurrentHealth = enemyData.maxHp;
            status.BaseAtkDamage = enemyData.damage;
            status.knockBack = enemyData.knockBack;
            status.expAmount = enemyData.expAmount;
        }
        else
        {
            Debug.LogError("Enemy Data is not assigned in " + gameObject.name);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocityY + knockback.y);
    }

    public void OnDead()
    {
        itemDropHelper.DropItem(transform.position, () => { gameObject.SetActive(false); });

        // 퀘스트 관련 이벤트
    }

    #region AIFunction
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
    #endregion
}
