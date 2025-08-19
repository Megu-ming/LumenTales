using Assets.Scripts.Utils;
using UnityEngine;

public class Status : MonoBehaviour
{
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

    public void Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage; 
            isInvincible=true;
        }
    }
}
