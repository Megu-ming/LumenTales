using System.Collections;
using UnityEngine;

public class EquipmentItem : Item
{
    public ArmorItemData ArmorData { get;private set; }

    public EquipmentItem(ArmorItemData data) : base(data)
    {
        ArmorData = data;
    }
}
