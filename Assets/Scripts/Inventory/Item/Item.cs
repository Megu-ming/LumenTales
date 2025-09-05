using System.Collections;
using UnityEngine;

public abstract class Item
{
    public ItemData itemData { get; private set; }

    protected Item(ItemData itemData)
    {
        this.itemData = itemData;
    }
}
