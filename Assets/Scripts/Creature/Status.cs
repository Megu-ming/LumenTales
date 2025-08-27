using System;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;

    [Header("HPBar")]
    public RectTransform hpBar;
    public GameObject barPrefab;
    protected GameObject hpBarInstance;
    public Canvas canvas;
    public float barHeight;

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
            {
                IsAlive = false;
                hpBarInstance.SetActive(false);
            }
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

        canvas = FindAnyObjectByType<Canvas>();
        hpBarInstance = Instantiate(barPrefab, canvas.transform);
        hpBar = hpBarInstance.GetComponent<RectTransform>();
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

        HPBar bar = hpBarInstance.GetComponent<HPBar>();
        bar.curHp = Health;
        bar.maxHp = MaxHealth;
    }

    private void FixedUpdate()
    {
        Vector3 barPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + barHeight, 0));
        hpBar.position = barPos;
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
