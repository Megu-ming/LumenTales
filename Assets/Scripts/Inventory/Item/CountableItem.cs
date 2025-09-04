using System.Collections;
using UnityEngine;

public class CountableItem : Item
{
    public CountableItemData CountableData { get;private set; }

    public int Amount { get; private set; }

    public int MaxAmount => CountableData.MaxAmount;
    public bool IsMax => Amount >= CountableData.MaxAmount;
    public bool IsEmpty => Amount <= 0;

    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        CountableData = data;
        SetAmount(1);
    }

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Amount + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxAmount) ? (nextAmount - MaxAmount) : 0;
    }
}
