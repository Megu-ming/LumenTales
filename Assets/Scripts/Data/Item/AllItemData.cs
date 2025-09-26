using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemDatabase")]
public class AllItemData : ScriptableObject, IItemResolver
{
    [SerializeField] List<ItemData> allItems;

    public bool TryGetItemData(string itemName, out ItemData data)
    {
        data = allItems.Find(x => x && x.ItemName == itemName);
        return data != null;
    }
}