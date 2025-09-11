using UnityEngine;

public class PlayerStatus : Status
{
    [Header("�÷��̾� �ɷ�ġ")]
    [SerializeField] private int strength = 0;  // ��
    [SerializeField] private int agility = 0;  // ��ø
    [SerializeField] private int luck = 0; // ���
    public int Strength { get => strength; set { strength = Mathf.Max(0, value); } }
    public int Agility { get => agility; set { agility = Mathf.Max(0, value); } }
    public int Luck { get => luck; set { luck = Mathf.Max(0, value); } }

    [Header("Derived")]
    [SerializeField] private float moveSpeed; // ���� �̵� �ӵ�
    [SerializeField] private float dropRate;  // ���� �����
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }
    public float DropRate { get => dropRate; private set => dropRate = value; }

    [Header("Attribute Scaling")]
    [SerializeField] private int strAttackPerPoint = 2;     // �� 1 = ���ݷ� +2
    [SerializeField] private int strHpPerPoint = 10;    // �� 1 = �ִ�HP +10
    [SerializeField] private int agiAttackPerPoint = 1;     // ��ø 1 = ���ݷ� +1
    [SerializeField] private float agiMovePerPoint = 0.05f; // ��ø 1 = �̵��ӵ� +0.05
    [SerializeField] private float lukMovePerPoint = 0.02f; // ��� 1 = �̵��ӵ� +0.02
    [SerializeField] private float lukDropPerPoint = 0.01f; // ��� 1 = ����� +1%


    /// <summary>
    /// ���/�⺻ġ + �ɷ�ġ �����ϸ��� ������ "���� ����"�� �����Ѵ�.
    /// - baseAtk/Def/HP/Move/Drop: ��� �� ���ʰ��� ��
    /// </summary>
    public void ApplyFinalStats(int baseAtk, int baseDef, int baseHP, float baseMove, float baseDrop)
    {
        // �� �ɷ�ġ ������ ���
        int addAtkFromStr = Strength * strAttackPerPoint;
        int addHpFromStr = Strength * strHpPerPoint;

        int addAtkFromAgi = Agility * agiAttackPerPoint;
        float addSpdFromAgi = Agility * agiMovePerPoint;

        float addSpdFromLuk = Luck * lukMovePerPoint;
        float addDropFromLuk = Luck * lukDropPerPoint;

        // �� ���� �ռ�
        Damage = baseAtk + addAtkFromStr + addAtkFromAgi;  // ���� ���ݷ�
        Defense = baseDef;                                   // ����(�ʿ� �� Str/Agi �ݿ� ����)
        MaxHealth = baseHP + addHpFromStr;                    // ���� �ִ�HP

        MoveSpeed = Mathf.Max(0f, baseMove + addSpdFromAgi + addSpdFromLuk);
        DropRate = Mathf.Clamp01(baseDrop + addDropFromLuk);

        // ���� ü���� �ִ�ġ�� ���� �ʰ�(����)
        Health = Mathf.Min(Health, MaxHealth);
    }

    // ��� ó��
    protected override void OnDied()
    {
        base.OnDied();
    }
}
