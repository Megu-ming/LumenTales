using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    // 공용 이벤트
    public UnityEvent<float, Vector2> damageableHit;

    [Header("상태")]
    [SerializeField] int level = 1;
    public int Level
    {
        get { return level; }
        set { level = Mathf.Max(1, value); }
    }
    [SerializeField] private float baseAtkDamage;
    public float BaseAtkDamage
    {
        get { return baseAtkDamage; }
        set { baseAtkDamage = value; }
    }
    public Vector2 knockBack = Vector2.zero;
    [SerializeField] private float baseDefence;
    public float BaseDefense
    {
        get { return baseDefence; }
        set { baseDefence = value; }
    }
    [SerializeField] private float baseMaxHealth;
    public float BaseMaxHealth
    {
        get { return baseMaxHealth; }
        set
        {
            baseMaxHealth = value;
        }
    }
    [SerializeField] private float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set 
        {
            currentHealth = Mathf.Clamp(value, 0, BaseMaxHealth);
            if (currentHealth == 0)
            {
                IsAlive = false;
                OnDied();
            }
        }
    }
    [SerializeField] private bool isAlive = true;
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
    public void SetInvincible(bool val) => isInvincible = val;
    [SerializeField] private float invincibilityTime = 0.25f;
    public void SetInvincibleTime(float time) => invincibilityTime = time;
    private float timeSinceHit = 0f;

    [SerializeField] Animator animator;

    public virtual void Init()
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

    /// <summary>
    /// 피격 됐을 때 상대방에서 호출
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="knockback"></param>
    /// <returns></returns>
    public virtual bool Hit(float damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            float finalDamage = Mathf.Max(0, damage); // 방어력 적용
            CurrentHealth -= finalDamage; 
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
        CurrentHealth = BaseMaxHealth;
        IsAlive = true;
    }

    protected virtual void OnDied() { }
}