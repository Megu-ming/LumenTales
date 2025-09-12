using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Items/Consumable")]
public class ConsumableItemData : CountableItemData
{
    [Header("Effect")]
    [TextArea] public string effectDesc = "Head 20 HP";

    public virtual void ApplyEffect(InventoryController owner)
    {
        Debug.Log($"Apply Effect: {ItemName} -> {effectDesc}");
    }

    public override Item CreateItem(int amount =1)
    {
        return new ConsumableItem(this, amount);
    }
}