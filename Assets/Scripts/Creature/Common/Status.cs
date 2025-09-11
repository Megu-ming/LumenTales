using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    // 공용 이벤트
    public UnityEvent<int, Vector2> damageableHit;

    [Header("상태")]
    [SerializeField] int level = 1;
    public int Level
    {
        get { return level; }
        set { level = Mathf.Max(1, value); }
    }
    [SerializeField] private int damage;
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    public Vector2 knockBack = Vector2.zero;
    [SerializeField] private int defense;
    public int Defense
    {
        get { return defense; }
        set { defense = value; }
    }
    [SerializeField] private int maxHealth;
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
        }
    }
    [SerializeField] private int health;
    public int Health
    {
        get { return health; }
        set 
        { 
            health = value;
            if (health <= 0)
            {
                IsAlive = false;
                OnDied();
            }
        }
    }
    [SerializeField] private bool isAlive;
    public bool IsAlive
    {
        get { return isAlive; }
        set 
        { 
            isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            if(value)
                gameObject.SetActive(true);
        }
    }

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set { animator.SetBool(AnimationStrings.lockVelocity, value); }
    }

    [Header("무적시간")]
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private float invincibilityTime = 0.25f;
    private float timeSinceHit = 0f;

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning($"{gameObject.name}의 자식 오브젝트에 Animator 컴포넌트가 없습니다.");
    }

    protected virtual void Start()
    {
        IsAlive = true;
    }

    protected virtual void Update()
    {
        if (isInvincible)
        {
            if(timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit +=Time.deltaTime;
        }
    }

    public virtual bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            int finalDamage = Mathf.Max(0, damage - defense); // 방어력 적용
            Health -= finalDamage; 
            isInvincible=true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;

            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }

    public virtual void Respawn()
    {
        Health = MaxHealth;
        IsAlive = true;
    }

    protected virtual void OnDied() { }
}