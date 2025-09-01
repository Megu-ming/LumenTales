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
        [Min(0)] public int weight; // 확률 기준값 (0~weightMax)
    }

    [Header("Drop Table")]
    public List<Items> items = new List<Items>();

    [Tooltip("확률 스케일. 100=퍼센트, 1000=퍼밀, 10000=베이시스포인트 등")]
    [SerializeField] private int weightMax = 100; // 100 = %

    /// <summary>
    /// 각 아이템을 독립적으로 roll 해서 드랍 목록을 반환.
    /// 운에 따라 0개~모든 아이템까지 나올 수 있음.
    /// </summary>
    public List<ItemData> PickDropsIndependent()
    {
        var result = new List<ItemData>();
        if (items == null || items.Count == 0 || weightMax <= 0) return result;

        foreach (var e in items)
        {
            if (e == null || e.item == null) continue;

            int clamped = Mathf.Clamp(e.weight, 0, weightMax);

            // Random.value(0~1) 대신 정수 범위로 명확하게 비교
            int roll = Random.Range(0, weightMax);
            if (roll < clamped)
            {
                result.Add(e.item);
            }
        }
        return result;
    }

    // (선택) 하나만 '룰렛'으로 뽑고 싶을 때 유틸
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

    // (선택) 중복 없이 N개를 '룰렛'으로 뽑고 싶을 때
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
                    pool.RemoveAt(i); // 중복 방지
                    break;
                }
                rnd -= e.weight;
            }
        }
        return picked;
    }
}
