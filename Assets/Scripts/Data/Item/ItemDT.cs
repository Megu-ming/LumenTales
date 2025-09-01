using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDT", menuName = "Data/ItemDT")]
public class ItemDT : ScriptableObject
{
    [System.Serializable]
    public class Items
    {
        public ItemData item;
        [Min(0)] public int weight; // Ȯ�� ���ذ� (0~weightMax)
    }

    [Header("Drop Table")]
    public List<Items> items = new List<Items>();

    [Tooltip("Ȯ�� ������. 100=�ۼ�Ʈ, 1000=�۹�, 10000=���̽ý�����Ʈ ��")]
    [SerializeField] private int weightMax = 100; // 100 = %

    /// <summary>
    /// �� �������� ���������� roll �ؼ� ��� ����� ��ȯ.
    /// � ���� 0��~��� �����۱��� ���� �� ����.
    /// </summary>
    public List<ItemData> PickDropsIndependent()
    {
        var result = new List<ItemData>();
        if (items == null || items.Count == 0 || weightMax <= 0) return result;

        foreach (var e in items)
        {
            if (e == null || e.item == null) continue;

            int clamped = Mathf.Clamp(e.weight, 0, weightMax);

            // Random.value(0~1) ��� ���� ������ ��Ȯ�ϰ� ��
            int roll = Random.Range(0, weightMax);
            if (roll < clamped)
            {
                result.Add(e.item);
            }
        }
        return result;
    }

    // (����) �ϳ��� '�귿'���� �̰� ���� �� ��ƿ
    public ItemData PickOneWeighted()
    {
        int sum = 0;
        foreach (var e in items) if (e != null) sum += Mathf.Max(0, e.weight);
        if (sum <= 0) return null;

        int rnd = Random.Range(0, sum); // [0, sum-1]
        foreach (var e in items)
        {
            if (e == null || e.weight <= 0) continue;
            if (rnd < e.weight) return e.item;
            rnd -= e.weight;
        }
        return null;
    }

    // (����) �ߺ� ���� N���� '�귿'���� �̰� ���� ��
    public List<ItemData> PickNWeightedNoRepeat(int n)
    {
        var pool = new List<Items>(items);
        var picked = new List<ItemData>();
        n = Mathf.Max(0, n);

        while (n-- > 0 && pool.Count > 0)
        {
            int sum = 0;
            foreach (var e in pool) if (e != null) sum += Mathf.Max(0, e.weight);
            if (sum <= 0) break;

            int rnd = Random.Range(0, sum);
            for (int i = 0; i < pool.Count; i++)
            {
                var e = pool[i];
                if (e == null || e.weight <= 0) continue;
                if (rnd < e.weight)
                {
                    picked.Add(e.item);
                    pool.RemoveAt(i); // �ߺ� ����
                    break;
                }
                rnd -= e.weight;
            }
        }
        return picked;
    }
}
