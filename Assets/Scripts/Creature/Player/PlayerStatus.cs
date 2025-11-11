using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerStatus : Status
{
    // 공통 능력치
    public override int Level 
    { 
        get => level; 
        set { level = Mathf.Max(1, value); }
    }

    public override float BaseAtkDamage  { get => baseAtkDamage; }
    public override float BaseMaxHealth  { get => baseMaxHealth; set => baseMaxHealth = value; }
    public override float CurrentHealth { get => currentHealth; }
    /// <summary>
    /// 현재 체력 설정
    /// </summary>
    /// <param name="val">설정할 현재 체력값</param>
    public void SetCurrentHealth(float val)
    {
        currentHealth = Mathf.Clamp(val, 0, FinalMaxHealth);
        HandleHpChanged?.Invoke(currentHealth, finalMaxHealth);
        if (currentHealth == 0)
        {
            IsAlive = false;
            OnDied();
        }
    }
    public void AddCurrentHealth(float val) => SetCurrentHealth(currentHealth + val);
    [SerializeField] float finalMaxHealth;

    [Header("플레이어 고유 기본 능력치")]
    [SerializeField] private float currentExp = 0;        // 경험치
    public float CurrentExp
    {
        get => currentExp;
        set
        {
            currentExp = Mathf.Clamp(value, 0, MaxExp);
            HandleExpChanged?.Invoke(CurrentExp, MaxExp);
        }
    }

    [SerializeField] private float maxExp = 10;           // 요구 경험치
    public float MaxExp { get => maxExp; set { maxExp = Mathf.Max(1, value); } }
    [SerializeField] private float baseStrength = 5;      // 힘
    [SerializeField] private float baseAgility = 5;       // 민첩

    [Header("장비 능력치")]
    [SerializeField] private float armorAddedAtk = 0;   // 장비로 추가된 공격력
    public float ArmorAddedAtk { get => armorAddedAtk; set => armorAddedAtk = value; }
    [SerializeField] private float armorAddedDef = 0;   // 장비로 추가된 방어력
    public float ArmorAddedDef { get => armorAddedDef; set => armorAddedDef = value; }
    [SerializeField] private float armorAddedStr = 0;   // 장비로 추가된 힘
    public float ArmorAddedStr { get => armorAddedStr; set => armorAddedStr = value; }
    [SerializeField] private float armorAddedAgi = 0;   // 장비로 추가된 민첩
    public float ArmorAddedAgi { get => armorAddedAgi; set => armorAddedAgi = value; }

    [Header("투자 포인트 능력치")]
    [SerializeField] private float spAddedStr = 0;      // 스탯포인트로 추가된 힘
    public float SpAddedStr { get => spAddedStr; set { spAddedStr = value; HandleHpChanged?.Invoke(CurrentHealth, FinalMaxHealth); } }
    [SerializeField] private float spAddedAgi = 0;      // 스탯포인트로 추가된 민첩
    public float SpAddedAgi { get => spAddedAgi; set { spAddedAgi = value; HandleHpChanged?.Invoke(CurrentHealth, FinalMaxHealth); } }

    // ------------------------- 최종 능력치 -------------------------
    /// <summary>
    /// 최종 데미지의 0.9~1.1배 사이로 랜덤한 데미지
    /// </summary>
    public float FinalRandomDamage         // 최종 공격력 = 기본공격력 + 장비공격력 + 힘스케일 + 민첩스케일
    { 
        get 
        {
            float strToAtk = Strength * strAttackPerPoint;
            float agiToAtk = Agility * agiAttackPerPoint;

            float centerDamage = BaseAtkDamage + armorAddedAtk + strToAtk + agiToAtk;

            float randDamage = UnityEngine.Random.Range(centerDamage * 0.9f, centerDamage * 1.1f);
            return randDamage;
        }
        set 
        { 
            float strToAtk = Strength * strAttackPerPoint;
            float agiToAtk = Agility * agiAttackPerPoint;

            finalAtkDamage = BaseAtkDamage + armorAddedAtk + strToAtk + agiToAtk; 
        } 
    }                                                         
    private float finalAtkDamage;

    public float FinalAttack { 
        get {
            float strToAtk = Strength * strAttackPerPoint;
            float agiToAtk = Agility * agiAttackPerPoint;

            float centerDamage = BaseAtkDamage + armorAddedAtk + strToAtk + agiToAtk;
            return centerDamage;
        } }
    
    public float Strength               // 최종 힘
    { 
        get => baseStrength + ArmorAddedStr + SpAddedStr;
        private set => finalStrength = baseStrength + ArmorAddedStr + SpAddedStr;
    }  
    [SerializeField] float finalStrength;
    public float Agility                // 최종 민첩
    { 
        get => baseAgility + ArmorAddedAgi + SpAddedAgi;
        private set => finalAgility = baseAgility + ArmorAddedAgi + SpAddedAgi; 
    }
    [SerializeField] float finalAgility;

    public float FinalMaxHealth { 
        get
        { 
            return finalMaxHealth = BaseMaxHealth + (SpAddedStr + ArmorAddedStr) * strHpPerPoint;
        } 
    }

    [Header("Attribute Scaling")]
    [SerializeField] private float strAttackPerPoint   = 0.25f;   // 힘 4 = 공격력 +1
    [SerializeField] private float strHpPerPoint       = 10;      // 힘 1 = 최대HP +10
    [SerializeField] private float agiAttackPerPoint   = 0.125f;  // 민첩 8 = 공격력 +1

    //Event Handler
    public Action HandleOpenDeadUI;
    public Action<float, float> HandleHpChanged;
    public Action<float, float> HandleExpChanged;

    public override void Init()
    {
        base.Init();
        CurrentHealth = FinalMaxHealth;
        HandleHpChanged?.Invoke(CurrentHealth, FinalMaxHealth);
    }

    // 사망 처리
    protected override void OnDied()
    {
        base.OnDied();

        HandleOpenDeadUI?.Invoke();
    }

    // 부활
    public override void Respawn()
    {
        SetCurrentHealth(FinalMaxHealth);
        IsAlive = true;

        SetInvincibleTime(2f);
        SetInvincible(true);
    }

    public void AddArmorAddedStat(EquipmentItemData data)
    {
        armorAddedAtk += data.attackValue;
        armorAddedDef += data.defenseValue;
        armorAddedStr += data.strength;
        armorAddedAgi += data.agility;
        SetCurrentHealth(currentHealth);
        CharacterEvents.infoUIRefresh?.Invoke();
    }

    public void RemoveArmorAddedStat(EquipmentItemData data)
    {
        armorAddedAtk -= data.attackValue;
        armorAddedDef -= data.defenseValue;
        armorAddedStr -= data.strength;
        armorAddedAgi -= data.agility;
        SetCurrentHealth(currentHealth);
        CharacterEvents.infoUIRefresh?.Invoke();
    }

    public override bool Hit(float damage, Vector2 knockback)
    {
        if(IsAlive && !isInvincible)
        {
            float finalDamage = Mathf.Max(0, damage);
            SetCurrentHealth(CurrentHealth - finalDamage);
            HandleHpChanged?.Invoke(CurrentHealth, FinalMaxHealth);
            isInvincible = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;

            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged?.Invoke(gameObject, finalDamage);

            return true;
        }

        return false;
    }
}
