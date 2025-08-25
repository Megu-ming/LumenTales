using System;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;

    [SerializeField] private int _maxHealth;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            _maxHealth = value;
        }
    }
    [SerializeField] private int _health;
    public int Health
    {
        get { return _health; }
        set 
        { 
            _health = value;
            if (_health <= 0)
                IsAlive = false;
        }
    }

    [SerializeField] private bool _isAlive;
    public bool IsAlive
    {
        get { return _isAlive; }
        set 
        { 
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive: " + value);
        }
    }

    public bool LockVelocity
    {
        get { return animator.GetBool(AnimationStrings.lockVelocity); }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    [SerializeField] private bool isInvincible = false;

    [SerializeField] private float invincibilityTime = 0.25f;
    private float timeSinceHit = 0f;

    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        IsAlive = true;
    }

    // Update is called once per frame
    void Update()
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

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage; 
            isInvincible=true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, damage);

            return true;
        }

        return false;
    }
}
