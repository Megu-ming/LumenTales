using System.Collections;
using UnityEngine;

public class ConsumableItem : CountableItem
{
    public ConsumableItemData ConsumableData { get;private set; }

    public ConsumableItem(ConsumableItemData data, int amount = 1) : base(data, amount)
    {
        ConsumableData = data;
        SetAmount(amount);
    }

    public bool Use(Player owner)
    {
        if(IsEmpty) return false;
        ConsumableData.ApplyEffect(owner);
        SetAmount(Amount - 1);
        return true;
    }
}
