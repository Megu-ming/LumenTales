using System.Collections.Generic;
using UnityEngine;

public class UIEquipment : MonoBehaviour
{
    [SerializeField] UIEquipmentSlot[] slots;

    // InventoryController에서 이벤트를 받을 때 사용
    private readonly Dictionary<EquipmentSlotType, UIEquipmentSlot> map = new();

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        Hide();
    }

    public void Init(InventoryController inv)
    {
        map.Clear();
        foreach (var s in slots)
        {
            if (s == null) continue;
            map[s.slotType] = s;
            s.Clear();
        }

        // 장착 변경 이벤트 구독
        inv.OnEquippedChanged += HandleEquippedChanged;

        // 초기 동기화(선택): inv.GetEquippedSnapshot()이 있다면 전체 반영
        foreach (var kv in inv.GetEquippedItems())
        {
            if (map.TryGetValue(kv.Key, out var ui)) ui.SetIcon(kv.Value.itemData.Icon);
        }
    }

    public void Dispose(InventoryController inv)
    {
        if (inv != null) inv.OnEquippedChanged -= HandleEquippedChanged;
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
}