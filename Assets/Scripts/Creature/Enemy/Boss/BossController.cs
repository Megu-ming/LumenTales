using System.Collections;
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
	ItemDropHelper dropHelper;

	// --------------------- 공격 가능 범위 ---------------------
	[Header("Ranges")]
	[SerializeField] float meleeAttackRange = 1.5f;
	[SerializeField] float laserAttackRange = 2.6f;
	[SerializeField] float minMissileAttackRange = 2f;
	[SerializeField] float maxMissileAttackRange = 5f;

	float targetDistance;

	// --------------------- 쿨타임 ---------------------
	[Header("쿨타임(초)")]
	[SerializeField] float attackCooltime = 3f;   // 모든 패턴 공통 딜레이(템포)
	[SerializeField] float meleeCooltime = 2f;    // 기본 공격 전용
	[SerializeField] float laserCooltime = 10f;
	[SerializeField] float missileCooltime = 15f;
	[SerializeField] float shieldCooltime = 20f;

	[Header("패턴 쿨타임 시각화(경과 시간)")]
	// 0에서 시작해서 각 쿨타임까지 증가(게이지로 쓰기 쉬움)
	[SerializeField] float attackTimer;  // 공통 게이트 타이머(경과)
	[SerializeField] float meleeTimer;
	[SerializeField] float laserTimer;
	[SerializeField] float missileTimer;
	[SerializeField] float shieldTimer;

	// 상태 플래그 (Ready = 사용 가능)
	bool canAttack = true;      // 공통 게이트
	bool isMeleeReady = true;
	bool isMissileReady = true;
	bool isLaserReady = true;
	bool isShieldReady = true;

	// 코루틴 핸들 (원하면 중복 방지/취소에 사용)
	Coroutine coAttack, coMelee, coLaser, coMissile, coShield;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponentInChildren<Animator>();
	}

	private void Start()
	{
		if (player == null && Player.instance != null)
			player = Player.instance.transform;
		if (status == null)
			status = GetComponent<BossStatus>();
		dropHelper = GetComponent<ItemDropHelper>();

		// 시작 시 타이머 초기화
		attackTimer = meleeTimer = laserTimer = missileTimer = shieldTimer = 0f;
		canAttack = isMeleeReady = isMissileReady = isLaserReady = isShieldReady = true;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void Update()
	{
		if (!player) return;

		// 공격 가능한 상태면 패턴 시도 (쿨타임 관리는 코루틴이 담당)
		TryTriggerPattern(targetDistance);
	}

	public void LookAtPlayer()
	{
		if (!player) return;
		if (transform.position.x > player.position.x)
			transform.localScale = new Vector3(-1, 1, 1);
		else
			transform.localScale = new Vector3(1, 1, 1);
	}

	/// <summary>
	/// 플레이어 방향을 바라보고 잠시 대기하여 공격 준비
	/// </summary>
	/// <returns>플레이어와 보스 사이 거리</returns>
	public float WaitForCalculateDistance()
	{
		targetDistance = Vector2.Distance(transform.position, player.position);
		// 대기
		StartCoroutine(Co_WaitTimer(5f));
		return targetDistance;		
	}

	public void MoveToPlayer()
	{
		if (!player) return;
		Vector2 target = new Vector2(player.position.x, rb.position.y);
		Vector2 newPos = Vector2.MoveTowards(rb.position, target, 1f * Time.fixedDeltaTime);
		rb.MovePosition(newPos);
	}

	// ======================= 패턴 선택 =======================
	public bool TryTriggerPattern(float distanceToPlayer)
	{
		if (!canAttack) return false;

		// 우선순위 예시: Shield > Missile > Laser(거리 체크) > Melee(거리 체크)
		if (isShieldReady)
			return Fire(BossSkillEntry.Shield);

		if (isMissileReady && 
			(distanceToPlayer >= minMissileAttackRange && distanceToPlayer <= maxMissileAttackRange))
			return Fire(BossSkillEntry.Missile);

		if (isLaserReady && distanceToPlayer <= laserAttackRange)
			return Fire(BossSkillEntry.Laser);

		if (isMeleeReady && distanceToPlayer <= meleeAttackRange)
			return Fire(BossSkillEntry.Default);

		return false;
	}

	// ======================= 발사/쿨타임 시작 =======================
	bool Fire(BossSkillEntry entry)
	{
		switch (entry)
		{
			case BossSkillEntry.Default:
				animator.SetTrigger("Attack");
				BeginCommonGate();
				isMeleeReady = false;
				meleeTimer = 0f;
				BeginCooldown(() => isMeleeReady = true, tick => meleeTimer = tick, meleeCooltime, ref coMelee);
				break;

			case BossSkillEntry.Laser:
				animator.SetTrigger("Laser");
				BeginCommonGate();
				isLaserReady = false;
				laserTimer = 0f;
				BeginCooldown(() => isLaserReady = true, tick => laserTimer = tick, laserCooltime, ref coLaser);
				break;

			case BossSkillEntry.Missile:
				animator.SetTrigger("Missile");
				BeginCommonGate();
				isMissileReady = false;
				missileTimer = 0f;
				BeginCooldown(() => isMissileReady = true, tick => missileTimer = tick, missileCooltime, ref coMissile);
				break;

			case BossSkillEntry.Shield:
				animator.SetTrigger("Shield");
				BeginCommonGate();
				isShieldReady = false;
				shieldTimer = 0f;
				BeginCooldown(()=> isShieldReady = true, tick => shieldTimer = tick , shieldCooltime, ref coShield);
				break;
		}

		return true;
	}

	// 공통 공격 템포 게이트 시작
	void BeginCommonGate()
	{
		canAttack = false;
		// 타이머 0부터 쌓이도록
		attackTimer = 0f;
		if (coAttack != null) StopCoroutine(coAttack);
		coAttack = StartCoroutine(Co_CooldownTimer(
			attackCooltime,
			tick => attackTimer = tick,
			() => canAttack = true
		));
	}

	// 개별 쿨다운 시작
	void BeginCooldown(System.Action onReady, System.Action<float> onTick, float cooldown, ref Coroutine handle)
	{
		if (handle != null) StopCoroutine(handle);
		handle = StartCoroutine(Co_CooldownTimer(
			cooldown,
			onTick,
			onReady
		));
	}

	// 쿨타임 코루틴 (프레임마다 경과시간 갱신 + 종료 시 Ready)
	IEnumerator Co_CooldownTimer(float cooldown, System.Action<float> onTick, System.Action onReady)
	{
		float t = 0f;
		while (t < cooldown)
		{
			t += Time.deltaTime;
			onTick?.Invoke(Mathf.Min(t, cooldown));
			yield return null; // 매 프레임 업데이트(게이지용)
		}
		onTick?.Invoke(cooldown);
		onReady?.Invoke();
	}

	// 대기 코루틴
	IEnumerator Co_WaitTimer(float waitTime)
	{
		float timer = 0f;
		while(timer <= waitTime)
		{
			timer += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	public void OnHit(float damage, Vector2 knockback)
	{
		// TODO: 경직/패턴 캔슬/분노 게이지 등 연동 지점
	}

    public override void OnDead()
    {
        dropHelper.DropItem(transform.position,() => gameObject.SetActive(false));
    }
}
