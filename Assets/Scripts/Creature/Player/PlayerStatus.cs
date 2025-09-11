using UnityEngine;

public class PlayerStatus : Status
{
    [Header("플레이어 능력치")]
    [SerializeField] private int strength = 0;  // 힘
    [SerializeField] private int agility = 0;  // 민첩
    [SerializeField] private int luck = 0; // 행운
    public int Strength { get => strength; set { strength = Mathf.Max(0, value); } }
    public int Agility { get => agility; set { agility = Mathf.Max(0, value); } }
    public int Luck { get => luck; set { luck = Mathf.Max(0, value); } }

    [Header("Derived")]
    [SerializeField] private float moveSpeed; // 최종 이동 속도
    [SerializeField] private float dropRate;  // 최종 드랍률
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    public float DropRate { get => dropRate; private set => dropRate = value; }

    [Header("Attribute Scaling")]
    [SerializeField] private int strAttackPerPoint = 2;     // 힘 1 = 공격력 +2
    [SerializeField] private int strHpPerPoint = 10;    // 힘 1 = 최대HP +10
    [SerializeField] private int agiAttackPerPoint = 1;     // 민첩 1 = 공격력 +1
    [SerializeField] private float agiMovePerPoint = 0.05f; // 민첩 1 = 이동속도 +0.05
    [SerializeField] private float lukMovePerPoint = 0.02f; // 행운 1 = 이동속도 +0.02
    [SerializeField] private float lukDropPerPoint = 0.01f; // 행운 1 = 드랍률 +1%


    /// <summary>
    /// 장비/기본치 + 능력치 스케일링을 적용해 "최종 스탯"을 갱신한다.
    /// - baseAtk/Def/HP/Move/Drop: 장비 및 기초값의 합
    /// </summary>
    public void ApplyFinalStats(int baseAtk, int baseDef, int baseHP, float baseMove, float baseDrop)
    {
        // ① 능력치 스케일 계산
        int addAtkFromStr = Strength * strAttackPerPoint;
        int addHpFromStr = Strength * strHpPerPoint;

        int addAtkFromAgi = Agility * agiAttackPerPoint;
        float addSpdFromAgi = Agility * agiMovePerPoint;

        float addSpdFromLuk = Luck * lukMovePerPoint;
        float addDropFromLuk = Luck * lukDropPerPoint;

        // ② 최종 합성
        Damage = baseAtk + addAtkFromStr + addAtkFromAgi;  // 최종 공격력
        Defense = baseDef;                                   // 방어력(필요 시 Str/Agi 반영 가능)
        MaxHealth = baseHP + addHpFromStr;                    // 최종 최대HP

        MoveSpeed = Mathf.Max(0f, baseMove + addSpdFromAgi + addSpdFromLuk);
        DropRate = Mathf.Clamp01(baseDrop + addDropFromLuk);

        // 현재 체력이 최대치를 넘지 않게(선택)
        Health = Mathf.Min(Health, MaxHealth);
    }

    // 사망 처리
    protected override void OnDied()
    {
        base.OnDied();
    }
}
