using System.Collections.Generic;
using UnityEngine;

public class UIEquipment : MonoBehaviour
{
    [SerializeField] UIEquipmentSlot[] slots;

    [SerializeField] InventoryController inventory;
    
    private readonly Dictionary<EquipmentSlotType, UIEquipmentSlot> map = new();

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        if (inventory == null) return;

        map.Clear();
        foreach(var s in slots)
        {
            if(s == null) continue;
            map[s.slotType] = s;
            s.Clear();
        }

        inventory.OnEquippedChanged += HandleEquippedChanged;

        foreach(var kv in inventory.GetEquippedItems())
        {
            if (map.TryGetValue(kv.Key, out var ui)) ui.SetIcon(kv.Value.itemData.Icon);
        }
    }

    private void OnDestroy()
    {
        if(inventory != null) inventory.OnEquippedChanged -= HandleEquippedChanged;
    }

    private void HandleEquippedChanged(EquipmentSlotType slot, EquipmentItem itemOrNull)
    {
        if (!map.TryGetValue(slot, out var ui)) return;
        
        if(itemOrNull == null) ui.SetIcon(null);
        else ui.SetIcon(itemOrNull.itemData.Icon);
    }

    public void OnToggleEquipment()
    {
        if (gameObject.activeSelf) Hide();
        else Show();
    }

    public void RequestUnequip(EquipmentSlotType slot)
    {
        inventory?.Unequip(slot);
    }
}