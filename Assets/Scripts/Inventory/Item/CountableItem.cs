using System.Collections;
using UnityEngine;

public class CountableItem : Item
{
    public CountableItemData CountableData { get;private set; }

    public int Amount { get; private set; }

    public int MaxAmount => CountableData.MaxAmount;
    public bool IsMax => Amount >= CountableData?.MaxAmount;
    public bool IsEmpty => Amount <= 0;

    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    public int AddAmount(int add)
    {
        int before = Amount;
        Amount = Mathf.Clamp(Amount + add, 0, MaxAmount);
        return Amount - before;
    }

    public int MergeUpToMax(int addAmount)
    {
        int space = CountableData.MaxAmount - Amount;
        int put = UnityEngine.Mathf.Clamp(addAmount, 0, space);
        Amount += put;
        return addAmount - put;
    }

    public bool CanMerge(CountableItem other)
    {
        return other != null && other.CountableData == this.CountableData;
    }
}
