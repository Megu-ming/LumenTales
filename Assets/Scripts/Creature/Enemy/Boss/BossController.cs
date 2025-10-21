using UnityEngine;
using UnityEngine.Events;

public class BossController : CreatureController
{
    Transform player;
    public EnemyStatus status;

    float attackRange = 1.5f;

    [Header("특수패턴 쿨타임")]
    [SerializeField] float attackCooltime = 3f;
    [SerializeField] float laserCooltime = 10f;
    [SerializeField] float missileCooltime = 15f;
    [SerializeField] float shieldCooltime = 20f;

    // "다음 사용 가능 시각"들
    float nextAttackTime;   // 글로벌 간격
    float nextLaserTime;
    float nextMissileTime;
    float nextShieldTime;

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
            status = GetComponent<EnemyStatus>();

        // 시작하자마자 다 사용 가능
        float t = Time.time;
        nextAttackTime = t;
        nextLaserTime = t;
        nextMissileTime = t;
        nextShieldTime = t;
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

    public bool TryTriggerNextPattern(float distanceToPlayer)
    {
        if (Time.time < nextAttackTime) return false;

        //if(Time.time >= nextShieldTime)
            //return Fire("Shield", ref nextShieldTime, shieldCooltime);
        if (Time.time >= nextMissileTime /* && 조건 */)
            return Fire("Missile", ref nextMissileTime, missileCooltime);
        //if (Time.time >= nextLaserTime /* && 조건 */)
            //return Fire("Laser", ref nextLaserTime, laserCooltime);

        // 특수패턴이 막혀있다면 기본공격(근접 범위일 때만)
        //if (distanceToPlayer <= attackRange)
            //return Fire("Attack", ref nextAttackTime, attackCooltime, isBasic: true);

        return false;
    }

    bool Fire(string triggerName, ref float nextPatternTime, float cooldown, bool isBasic = false)
    {
        animator.SetTrigger(triggerName);
        if (!isBasic) nextPatternTime = Time.time + cooldown; // 글로벌 쿨타임 적용
        nextAttackTime = Time.time + attackCooltime;
        return true;
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        
    }
}
