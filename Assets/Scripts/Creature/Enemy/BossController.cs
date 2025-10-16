using UnityEngine;
using UnityEngine.Events;

public class BossController : CreatureController
{
    [Header("Targeting")]
    [SerializeField] private Transform manualTarget; // Optional override
    [SerializeField] private float sightRange = 12f; // Acquire/keep target within this range
    [SerializeField] private float attackStopDistance = 2.0f; // Stop here (your attack script can take over)

    // === Combat (cooldowns & hooks) ===
    [Header("Combat")]
    [SerializeField] private float basicCooldown = 1.0f; // 기본 공격 쿨타임
    [SerializeField] private float specialCooldown = 6.0f; // 특수 공격 쿨타임
    [SerializeField] private float specialMinDistance = 3.0f; // 특수공격을 선호할 최소 거리 (예: 돌진/점프 등)
    [SerializeField] private float attackLock = 0.25f; // 공격 중 이동 잠금 시간(선딜)
    [SerializeField] private float postAttackHold = 0.1f; // 후딜 동안 이동 잠금 추가

    [SerializeField] private string basicAttackTrigger = "Attack"; // 애니메이터 트리거명 (선택)
    [SerializeField] private string specialAttackTrigger = "Skill"; // 애니메이터 트리거명 (선택)

    // 외부로 이벤트 노출 (VFX/사운드/히트박스 스폰 연결)
    public UnityEvent onBasicAttack;
    public UnityEvent onSpecialAttack;

    // 런타임 타이밍
    private float nextBasicTime; // Time.time 기준 재사용 가능 시점
    private float nextSpecialTime; // Time.time 기준 재사용 가능 시점
    private float lockTimer; // 이동 잠금 타이머
    private float attackDecisionTimer; // 공격 의사결정 간격 스로틀
    private const float ATTACK_DECIDE_INTERVAL = 0.15f;

    // === Movement ===
    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.6f;
    [SerializeField] private float chaseSpeed = 3.4f;
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 55f;

    // === Leash & Patrol ===
    [Header("Leash/Patrol")]
    [SerializeField] private float leashRadius = 15f; // Max distance from spawn while chasing
    [SerializeField] private bool usePatrol = true;
    [SerializeField] private float patrolRadius = 4f; // Left/right from spawn
    [SerializeField] private float patrolPause = 1.25f; // Pause at ends

    // === Ground/Wall Probes ===
    [Header("Probes")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float wallCheckDistance = 0.25f; // Forward ray to catch walls
    [SerializeField] private float ledgeCheckDistance = 0.35f; // Down ray at feet to avoid falling
    [SerializeField] private float groundProbeYOffset = 0.05f;

    // === Cached ===
    private Collider2D col;
    private Transform target;
    private Vector2 spawnPos;

    // Runtime
    private float desiredX; // desired horizontal speed (signed)
    private float waitTimer; // for patrol pauses
    private float retargetTimer; // throttle costly target searches
    private const float RETARGET_INTERVAL = 0.15f;

    private enum MoveState { Idle = 0, Patrol = 1, Chase = 2, Return = 3 }
    [SerializeField] private MoveState state = MoveState.Idle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        spawnPos = transform.position;
    }

    private void OnEnable()
    {
        state = usePatrol ? MoveState.Patrol : MoveState.Idle;
    }

    private void Update()
    {
        // Timer
        attackDecisionTimer -= Time.deltaTime;

        if(animator)
        {
            animator.SetFloat("SpeedX", Mathf.Abs(rb.linearVelocityX));
            animator.SetBool("Chasing", state == MoveState.Chase);
            animator.SetInteger("State", (int)state);
        }

        retargetTimer -= Time.deltaTime;
        if(retargetTimer <= 0f)
        {
            retargetTimer = RETARGET_INTERVAL;
            ResolveTarget();
        }

        switch(state)
        {
            case MoveState.Idle:
                desiredX = 0f;
                if (HasValidTargetInRange()) state = MoveState.Chase;
                else if (usePatrol) state = MoveState.Patrol;
                break;
            case MoveState.Patrol:
                PatrolTick();
                if (HasValidTargetInRange()) state = MoveState.Chase;
                break;
            case MoveState.Chase:
                ChaseTick();
                break;
            case MoveState.Return:
                ReturnTick();
                break;
        }
    }

    private void FixedUpdate()
    {
        // 이동 잠금(공격 선후딜)
        if (lockTimer > 0f)
            lockTimer -= Time.fixedDeltaTime;

        float currentX = rb.linearVelocityX;
        float accel = Mathf.Abs(desiredX) > Mathf.Abs(currentX) ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(currentX, desiredX, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocityY);
    }

    private void PatrolTick()
    {
        float left = spawnPos.x - patrolRadius;
        float right = spawnPos.x + patrolRadius;

        if(Mathf.Abs(desiredX)<0.01f)
        {
            desiredX = (transform.position.x < (left + right) * 0.5f) ? +patrolSpeed : -patrolSpeed;
        }

        if(waitTimer>0f)
        {
            waitTimer -= Time.deltaTime;
            desiredX = 0f;
            return;
        }

        if(ShouldTurn())
        {
            desiredX = -Mathf.Sign(desiredX) * patrolSpeed;
            waitTimer = patrolPause;
            return;
        }

        if(transform.position.x<=left && desiredX<0f)
        {
            desiredX = patrolSpeed;
            waitTimer = patrolPause;
        }
        else if(transform.position.x>=right && desiredX>0f)
        {
            desiredX = -patrolSpeed;
            waitTimer = patrolPause;
        }

        FaceBySpeed(desiredX);
    }

    private void ChaseTick()
    {
        if (!target)
        {
            state = MoveState.Return;
            return;
        }


        Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
        float dist = Mathf.Abs(toTarget.x);


        // 리쉬 범위 벗어나면 귀환
        if (Vector2.Distance(transform.position, spawnPos) > leashRadius)
        {
            state = MoveState.Return;
            desiredX = 0f;
            return;
        }


        // 공격 거리: 정지하고 공격 시도
        if (dist <= attackStopDistance)
        {
            desiredX = 0f;
            FaceByDelta(toTarget.x);


            if (attackDecisionTimer <= 0f)
            {
                attackDecisionTimer = ATTACK_DECIDE_INTERVAL;
                TryAttack(dist);
            }
            return; // 공격/대기
        }


        // 타겟을 향해 추격
        desiredX = Mathf.Sign(toTarget.x) * chaseSpeed;
        FaceBySpeed(desiredX);


        // 낭떠러지/벽 앞에서는 잠깐 멈춰서 판단
        if (ShouldTurn())
        {
            desiredX = 0f;
            if (attackDecisionTimer <= 0f)
            {
                attackDecisionTimer = ATTACK_DECIDE_INTERVAL;
                // 막혔을 때 특수공격(돌진/점프 등) 우선 시도
                TryAttack(dist, preferSpecialWhenBlocked: true);
            }
        }
    }

    private void ReturnTick()
    {
        float dx = spawnPos.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.05f)
        {
            transform.position = new Vector3(spawnPos.x, transform.position.y, transform.position.z);
            desiredX = 0f;
            state = usePatrol ? MoveState.Patrol : MoveState.Idle;
            return;
        }

        desiredX = Mathf.Sign(dx) * patrolSpeed;
        FaceBySpeed(desiredX);

        if (ShouldTurn()) desiredX = 0f;
    }

    private bool TryAttack(float horizontalDistance, bool preferSpecialWhenBlocked = false)
    {
        float now = Time.time;
        bool canBasic = now >= nextBasicTime;
        bool canSpecial = now >= nextSpecialTime && horizontalDistance >= specialMinDistance;

        if (preferSpecialWhenBlocked && canSpecial)
            return TriggerSpecial(now);
        if (canSpecial)
            return TriggerSpecial(now);
        else if (canBasic)
            return TriggerBasic(now);

        return false;
    }

    private bool TriggerBasic(float now)
    {
        lockTimer = Mathf.Max(lockTimer, attackLock + postAttackHold);

        if (animator && !string.IsNullOrEmpty(basicAttackTrigger)) animator.SetTrigger(basicAttackTrigger);
        onBasicAttack?.Invoke();

        nextBasicTime = now + basicCooldown;
        return true;
    }

    private bool TriggerSpecial(float now)
    {
        lockTimer = Mathf.Max(lockTimer, attackLock + postAttackHold);
        if (animator && !string.IsNullOrEmpty(specialAttackTrigger)) animator.SetTrigger(specialAttackTrigger);
        onSpecialAttack?.Invoke();

        nextSpecialTime = now + specialCooldown;
        return true;
    }

    private void ResolveTarget()
    {
        if(manualTarget)
        {
            target = manualTarget;
            return;
        }

        if(Player.instance)
        {
            target = Player.instance.transform;
            return;
        }
    }

    private bool HasValidTargetInRange()
    {
        if (target is null) return false;
        if (Vector2.Distance(transform.position, target.position) > sightRange) return false;
        return true;
    }

    private bool ShouldTurn()
    {
        if (!col) return false;
        Bounds b = col.bounds;
        float dir = Mathf.Sign(desiredX == 0f ? (transform.localScale.x > 0f ? 1f : -1f) : desiredX);

        // Wall Check
        Vector2 wallOrigin = new Vector2(dir > 0 ? b.max.x : b.min.x, b.center.y);
        bool wall = Physics2D.Raycast(wallOrigin, Vector2.right * dir, wallCheckDistance, groundMask);

        return wall;
    }

    private void FaceBySpeed(float xSpeed)
    {
        if (Mathf.Abs(xSpeed) < 0.01f) return;
        var ls = transform.localScale;
        ls.x = Mathf.Abs(ls.x) * (xSpeed > 0 ? 1f : -1f);
        transform.localScale = ls;
    }

    private void FaceByDelta(float dx)
    {
        if (Mathf.Abs(dx) < 0.01f) return;
        var ls = transform.localScale;
        ls.x = Mathf.Abs(ls.x) * (dx > 0 ? 1f : -1f);
        transform.localScale = ls;
    }

    private Vector2 GetHeadPoint()
    {
        if (!col) return transform.position;
        Bounds b = col.bounds;
        return new Vector2(b.center.x, b.max.y - 0.05f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)spawnPos : transform.position, leashRadius);
        Gizmos.color = new Color(1f, 0.6f, 0f, 1f); Gizmos.DrawWireSphere(transform.position, sightRange);
        if (usePatrol)
        {
            Gizmos.color = Color.yellow;
            var p = Application.isPlaying ? (Vector3)spawnPos : transform.position;
            Gizmos.DrawLine(new Vector3(p.x - patrolRadius, p.y + 0.5f), new Vector3(p.x + patrolRadius, p.y + 0.5f));
        }


        if (col)
        {
            Bounds b = col.bounds;
            float dir = Mathf.Sign(transform.localScale.x);
            Vector3 wallOrigin = new Vector3(dir > 0 ? b.max.x : b.min.x, b.center.y);
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(wallOrigin, wallOrigin + Vector3.right * dir * wallCheckDistance);


            Vector3 footFront = new Vector3(dir > 0 ? b.max.x : b.min.x, b.min.y + groundProbeYOffset);
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawLine(footFront, footFront + Vector3.down * ledgeCheckDistance);
        }
    }
#endif
}
