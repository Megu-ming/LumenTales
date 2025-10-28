using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : Status
{
    [Header("플레이어 기본 능력치")]
    [SerializeField] private float currentExp = 0;        // 경험치
    [SerializeField] private float maxExp = 10;           // 요구 경험치
    [SerializeField] private float baseStrength = 5;      // 힘
    [SerializeField] private float baseAgility = 5;       // 민첩
    [SerializeField] private float baseLuck = 5;          // 행운

    [Header("장비 능력치")]
    [SerializeField] private float armorAddedAtk = 0;   // 장비로 추가된 공격력
    public float ArmorAddedAtk { get => armorAddedAtk; set => armorAddedAtk = value; }
    [SerializeField] private float armorAddedDef = 0;   // 장비로 추가된 방어력
    public float ArmorAddedDef { get => armorAddedDef; set => armorAddedDef = value; }
    [SerializeField] private float armorAddedStr = 0;   // 장비로 추가된 힘
    public float ArmorAddedStr { get => armorAddedStr; set => armorAddedStr = value; }
    [SerializeField] private float armorAddedAgi = 0;   // 장비로 추가된 민첩
    public float ArmorAddedAgi { get => armorAddedAgi; set => armorAddedAgi = value; }
    [SerializeField] private float armorAddedLuk = 0;   // 장비로 추가된 행운
    public float ArmorAddedLuk { get => armorAddedLuk; set => armorAddedLuk = value; }

    [Header("투자 포인트 능력치")]
    [SerializeField] private float spAddedStr = 0;      // 스탯포인트로 추가된 힘
    public float SpAddedStr { get => spAddedStr; set => spAddedStr = value; }
    [SerializeField] private float spAddedAgi = 0;      // 스탯포인트로 추가된 민첩
    public float SpAddedAgi { get => spAddedAgi; set => spAddedAgi = value; }
    [SerializeField] private float spAddedLuk = 0;      // 스탯포인트로 추가된 행운
    public float SpAddedLuk { get => spAddedLuk; set => spAddedLuk = value; }

    public float CurrentExp { get => currentExp; set { currentExp = Mathf.Clamp(value, 0, MaxExp); HandleExpChanged?.Invoke(CurrentExp, MaxExp); } }
    public float MaxExp { get => maxExp; set { maxExp = Mathf.Max(1, value); } }

    public float FinalAtkDamage         // 최종 공격력 = 기본공격력 + 장비공격력 + 힘스케일 + 민첩스케일
    { 
        get 
        {
            float strToAtk = Strength * strAttackPerPoint;
            float agiToAtk = Agility * agiAttackPerPoint;

            return BaseAtkDamage + armorAddedAtk + strToAtk + agiToAtk;
        }
        set 
        { 
            float strToAtk = Strength * strAttackPerPoint;
            float agiToAtk = Agility * agiAttackPerPoint;

            finalAtkDamage = BaseAtkDamage + armorAddedAtk + strToAtk + agiToAtk; 
        } 
    }                                                         
    private float finalAtkDamage;
    
    public float Strength               // 최종 힘
    { 
        get => baseStrength + ArmorAddedStr + SpAddedStr;
        private set => finalStrength = baseStrength + ArmorAddedStr + SpAddedStr;
    }  
    private float finalStrength;
    public float Agility                // 최종 민첩
    { 
        get => baseAgility + ArmorAddedAgi + SpAddedAgi;
        private set => finalAgility = baseAgility + ArmorAddedAgi + SpAddedAgi; 
    }     
    private float finalAgility;
    public float Luck                    // 최종 행운
    { 
        get => baseLuck + ArmorAddedLuk + SpAddedLuk;
        private set => finalLuck = baseLuck + ArmorAddedLuk + SpAddedLuk; 
    }             
    private float finalLuck;

    public float FinalMaxHealth
    {
        get => BaseMaxHealth + (SpAddedStr + ArmorAddedStr) * strHpPerPoint;
    }
    private float finalMaxHealth;

    [Header("Derived")]

    [Header("Attribute Scaling")]
    [SerializeField] private float strAttackPerPoint   = 0.25f;   // 힘 4 = 공격력 +1
    [SerializeField] private float strHpPerPoint       = 10;      // 힘 1 = 최대HP +10
    [SerializeField] private float agiAttackPerPoint   = 0.125f;  // 민첩 8 = 공격력 +1
    [SerializeField] private float lukAttackPerPoint   = 0.125f;  // 행운 8 = 공격력 +1
    [SerializeField] private float lukDefPerPoint      = 0.5f;    // 행운 2 = 방어력 +1

    //Event Handler
    public Action HandleOpenDeadUI;
    public Action<float, float> HandleExpChanged;

    public void Init()
    {
        CurrentHealth = FinalMaxHealth;
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
        base.Respawn();
    }

    public void AddArmorAddedStat(EquipmentItemData data)
    {
        armorAddedAtk += data.attackValue;
        armorAddedDef += data.defenseValue;
        armorAddedStr += data.strength;
        armorAddedAgi += data.agility;
        armorAddedLuk += data.luck;

        CharacterEvents.infoUIRefresh?.Invoke();
    }

    public void RemoveArmorAddedStat(EquipmentItemData data)
    {
        armorAddedAtk -= data.attackValue;
        armorAddedDef -= data.defenseValue;
        armorAddedStr -= data.strength;
        armorAddedAgi -= data.agility;
        armorAddedLuk -= data.luck;

        CharacterEvents.infoUIRefresh?.Invoke();
    }
}
