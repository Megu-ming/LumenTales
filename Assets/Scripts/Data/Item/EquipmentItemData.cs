using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Items/Armor")]
public class EquipmentItemData : ItemData
{
    public EquipmentSlotType slot;
    public int defenseValue = 5;

    public override bool IsStackable => false;

    public override Item CreateItem(int amount =1)
    {
        return new EquipmentItem(this);
    }
}