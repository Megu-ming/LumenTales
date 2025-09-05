using UnityEngine;

[CreateAssetMenu(fileName = "New Countable Item", menuName = "Inventory/Items/Countable Item")]
public class CountableItemData : ItemData
{
    public int MaxAmount => maxAmount;
    [SerializeField] private int maxAmount = 99;

    public override bool IsStackable => true;

    public override Item CreateItem(int amount =1)
    {
        return new CountableItem(this, amount);
    }
}