using UnityEngine;
using UnityEngine.Events;

public enum BossSkillEntry
{
    Default,
    Missile,
    Laser,
    Shield,
}

public class BossController : CreatureController
{
    Transform player;
    public BossStatus status;

    float attackRange = 1.5f;

    [Header("특수패턴 쿨타임")]
    [SerializeField] float attackCooltime = 3f;
    [SerializeField] float meleeCooltime = 2f;
    [SerializeField] float laserCooltime = 10f;
    [SerializeField] float missileCooltime = 15f;
    [SerializeField] float shieldCooltime = 20f;

    [Header("패턴 쿨타임 시각화")]
    // "다음 사용 가능 시각"들
    [SerializeField] float attackTimer;   // 글로벌 간격
    [SerializeField] float meleeTimer;
    [SerializeField] float laserTimer;
    [SerializeField] float missileTimer;
    [SerializeField] float shieldTimer;

    bool canAttack = true;
    bool isMeleeReady = true;
    bool isMissileReady = true;
    bool isLaserReady = true;
    bool isShieldReady = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (player == null && Player.instance != null)
        {
            player = Player.instance.transform;
        }
        if(status is null)
            status = GetComponent<BossStatus>();
    }

    private void Update()
    {
        // 공격 가능해지면 타이머 안돌림
        if (canAttack is false) 
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooltime)
                canAttack = true;
        }

        if (isMeleeReady is false)
        {
            meleeTimer += Time.deltaTime;
            if(meleeTimer >= meleeCooltime)
                isMeleeReady = true;
        }

        // 나머지 특수 패턴들은 패턴 사용가능한 상태가 아니면 항상 쿨타임이 돔
        if(isMissileReady is false)
        {
            missileTimer += Time.deltaTime;
            if(missileTimer >= missileCooltime)
                isMissileReady = true;
        }
        if (isLaserReady is false)
        {
            laserTimer += Time.deltaTime;
            if(laserTimer >= laserCooltime)
                isLaserReady = true;
        }
        if (isShieldReady is false)
        {
            shieldTimer += Time.deltaTime;
            if(shieldTimer >= shieldCooltime)
                isShieldReady = true;
        }

        // 공격 가능한 상태면 패턴 사용
        TryTriggerPattern();
    }

    public void LookAtPlayer()
    {
        if(transform.position.x > player.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    public void MoveToPlayer()
    {
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, 1f * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    public bool TryTriggerPattern(/*float distanceToPlayer*/)
    {
        if (canAttack is false) return false;

        if (isShieldReady is true)
            return Fire("Shield", ref shieldTimer, ref isShieldReady);
        if (isMissileReady is true)
            return Fire("Missile", ref missileTimer, ref isMissileReady);
        if (isLaserReady is true)
            return Fire("Laser", ref laserTimer, ref isLaserReady);

        // 전부 쿨타임이면 평타
        return Fire("Attack", ref attackTimer, ref isMeleeReady);
    }

    bool Fire(string triggerName, ref float patternTimer, ref bool isPatternReady)
    {
        animator.SetTrigger(triggerName);
        patternTimer = 0f;

        // 여기 수정해야함!!
        
        return true;
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        
    }
}
