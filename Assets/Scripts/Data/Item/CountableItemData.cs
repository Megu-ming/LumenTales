using UnityEngine;

[CreateAssetMenu(fileName = "New Countable Item", menuName = "Inventory/Items/Countable Item")]
public class CountableItemData : ItemData
{
    public int MaxAmount => maxAmount;
    [SerializeField] private int maxAmount = 99;

    public override Item CreateItem()
    {
        return new CountableItem(this);
    }
}