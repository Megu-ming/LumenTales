using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody2D),typeof(TouchingDirections))]
public class EnemyController : CreatureController
{
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

    [Header("ItemDrop")]
    [SerializeField] float fanHalfAngle = 35f;   // 부채꼴 반 각도(도). 예: 35 → 좌90+35, 우90-35
    [SerializeField] float baseSpeed = 4.5f;     // 초기 속도
    [SerializeField] float speedJitter = 1.0f;   // 속도 랜덤(±)
    [SerializeField] float staggerMin = 0.03f;   // 아이템 사이 지연
    [SerializeField] float staggerMax = 0.10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        status = GetComponent<Status>();
        touchingDirections = GetComponent<TouchingDirections>();

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
            status.MaxHealth = enemyData.maxHp;
            status.Health = enemyData.maxHp;
            status.Damage = enemyData.damage;
            status.knockBack = enemyData.knockBack;

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
        var items = enemyData.itemDT.PickItem(1);

        DropItems(items);
        gameObject.SetActive(false);
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

    #region ItemDropFunction
    protected void DropItems(List<ItemData> items)
    {
        StartCoroutine(DropRoutine(items, transform.position));
    }

    private IEnumerator DropRoutine(List<ItemData> items, Vector3 position)
    {
        int count = items.Count;
        if(count == 0) yield break;

        for (int i = 0; i < count; i++)
        {
            var data = items[i];

            float angleDeg = GetAngleForIndex(i, count);

            float spd = baseSpeed + Random.Range(-speedJitter, speedJitter);
            Vector2 v0 = AngleToVelocity(angleDeg, spd);

            var go = Instantiate(data.prefab, position, Quaternion.identity);
            var item = go.GetComponent<Item>();
            item?.Init(data);

            var rb = go.GetComponent<Rigidbody2D>();
            var col = go.GetComponent<Collider2D>();
            if (rb)
            {
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                rb.linearVelocity = v0;
            }

            float wait = Random.Range(staggerMin, staggerMax);
            if (wait > 0f) yield return new WaitForSeconds(wait);
            else yield return null;
        }

    }

    private float GetAngleForIndex(int index, int count)
    {
        if (count <= 1) return 90f;

        float leftDeg = 90f + fanHalfAngle;
        float rightDeg = 90f - fanHalfAngle;

        if (count == 2)
        {
            return (index == 0) ? leftDeg : rightDeg; // 왼쪽부터 순서대로
        }

        // 3개 이상: 균등 분배 (0 → left, count-1 → right)
        float t = (float)index / (count - 1);
        return Mathf.Lerp(leftDeg, rightDeg, t);
    }

    private Vector2 AngleToVelocity(float angleDeg, float speed)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad) * speed, Mathf.Sin(rad) * speed);
    }

    #endregion
}
