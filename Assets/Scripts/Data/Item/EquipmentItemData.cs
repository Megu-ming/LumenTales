using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Items/Armor")]
public class EquipmentItemData : ItemData
{
    public EquipmentSlotType slot;
    public int defenseValue = 5;
    public int attackValue = 0;
    public bool isArmor = true;

    public int strength = 0;    // 힘
    public int agility = 0;     // 민첩
    public int luck = 0;        // 행운

    public override bool IsStackable => false;

    public override Item CreateItem(int amount =1)
    {
        return new EquipmentItem(this);
    }
}