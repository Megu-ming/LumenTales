using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Items/Armor")]
public class EquipmentItemData : ItemData
{
    public EquipmentSlotType slot;
    public int defenseValue = 5;
    public int attackValue = 0;
    public bool isArmor = true;
    public override bool IsStackable => false;

    public override Item CreateItem(int amount =1)
    {
        return new EquipmentItem(this);
    }
}