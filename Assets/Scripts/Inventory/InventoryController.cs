using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public int Capacity { get; private set; }
    [SerializeField, Range(10, 30)] int initialCapacity = 30;
    [SerializeField, Range(0, 30)] int maxInventorySize = 30;

    // 실제 아이템 데이터
    private Item[] items;

    // 장비 슬롯
    private readonly Dictionary<EquipmentSlotType, EquipmentItem> equipped = new();
    public event Action<EquipmentSlotType, EquipmentItem> OnEquippedChanged;

    public int Gold { get; private set; }
    public event Action<int> OnGoldChanged;

    [SerializeField] PlayerStatus playerStatus;

    [SerializeField] private int baseAttack = 10;
    [SerializeField] private int baseDefense = 0;
    [SerializeField] private int baseMaxHP = 100;
    [SerializeField] private float baseMoveSpd = 5.0f;
    [SerializeField] private float baseDropRate = 0.05f; // 5%

    public event Action<int, ItemData> OnSlotUpdated;

    private void Awake()
    {
        items = new Item[maxInventorySize];
        Capacity = initialCapacity;

        RefreshAllSlots();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var world = collision.GetComponent<WorldItem>();
        if (!world || world.itemData == null) return;

        int remainder = Add(world.itemData, world.amount);

        if(remainder<=0) world.CollectItem(transform);
        else world.amount = remainder;
    }

    #region Public Methods
    public bool HasItem(int index) => IsValidIndex(index) && items[index] != null;
    public bool IsCountableItem(int index) => HasItem(index) && items[index] is CountableItem;

    public ItemData GetItemData(int index) => HasItem(index) ? items[index].itemData : null;
    public string GetItemName(int index) => GetItemData(index) ? GetItemData(index).ItemName : string.Empty;

    public int GetCurrentAmount(int index)
    {
        if (!IsCountableItem(index)) return 0;
        return ((CountableItem)items[index]).Amount;
    }

    public IReadOnlyDictionary<EquipmentSlotType, EquipmentItem> GetEquippedItems() => equipped;

    public void EquipFromInventory(int index, EquipmentSlotType tartgetType)
    {
        if (!IsValidIndex(index)) return;
        if (items[index] is not EquipmentItem eq) return;

        if(equipped.TryGetValue(tartgetType, out var current) && current != null)
        {
            int empty = FindEmptySlotIndex();
            if (empty == -1)
            {
                Debug.LogWarning("No empty slot to unequip current armor.");
                return;
            }
            items[empty] = current;
            UpdateSlot(empty);
        }

        items[index] = null;
        UpdateSlot(index);
        equipped[tartgetType] = eq;
        OnEquippedChanged?.Invoke(tartgetType, eq);
    }

    public void Unequip(EquipmentSlotType slot)
    {
        if (!equipped.TryGetValue(slot, out var current) || current == null) return;
        
        int empty = FindEmptySlotIndex();
        if (empty == -1)
        {
            Debug.LogWarning("No empty slot to unequip current armor.");
            return;
        }

        items[empty] = current;
        UpdateSlot(empty);
        equipped[slot] = null;
        OnEquippedChanged?.Invoke(slot, null);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        Gold += amount;
        RaiseGoldChanged();
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (amount > Gold) return false;
        Gold -= amount;
        RaiseGoldChanged();
        return true;
    }

    public int Add(ItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0) return 0;
        if(itemData is GoldData gd)
        {
            AddGold(gd.amount);
            return 0;
        }

        if (itemData.IsStackable)
        {
            // 같은 스택에 병합 먼저
            int remain = amount;
            for(int i=0;i<Capacity&&remain>0;i++)
            {
                if (items[i] is CountableItem ci && ci.CountableData == (CountableItemData)itemData)
                {
                    remain = ci.MergeUpToMax(remain);
                    UpdateSlot(i);
                }
            }

            // 남았으면 빈칸에 새 스택 생성
            while (remain > 0)
            {
                int slot = FindEmptySlotIndex();
                if(slot == -1) break;

                int put = Mathf.Min(remain, ((CountableItemData)itemData).MaxAmount);
                items[slot] = new CountableItem((CountableItemData)itemData, put);
                UpdateSlot(slot);
                remain -= put;
            }
            return remain; // 남은 수량 반환
        }
        else
        {
            // 스택 x 아이템 : amount 개수만큼 빈칸에 추가
            int remain = amount;
            while (remain > 0)
            {
                int slot = FindEmptySlotIndex();
                if (slot == -1) break;

                items[slot] = itemData.CreateItem();
                UpdateSlot(slot);
                remain--;
            }
            return remain;
        }
    }

    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;
        items[index] = null;
        UpdateSlot(index);
    }

    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        Item itemA = items[indexA];
        Item itemB = items[indexB];

        // 셀 수 있고 동일한 아이템이면 A -> B로 개수 합치기
        if (itemA is CountableItem ca && itemB is CountableItem cb && ca.CanMerge(cb))
        {
            int remain = cb.MergeUpToMax(ca.Amount);
            ca.SetAmount(remain);
            if (ca.IsEmpty) items[indexA] = null;
        }
        else
        {
            items[indexA] = itemB;
            items[indexB] = itemA;
        }

        UpdateSlot(indexA); UpdateSlot(indexB);
    }

    public void UseAt(int index)
    {
        if(!IsValidIndex(index) || items[index] == null) return;

        if (items[index] is ConsumableItem con)
        {
            bool used = con.Use(this);
            if (used)
            {
                if(con.IsEmpty) items[index] = null;
                UpdateSlot(index);
            }
        }
        else if (items[index] is EquipmentItem eq)
        {
            ToggleEquip(index, eq);
        }
    }

    public bool TryTransfer(SlotAddress from, SlotAddress to)
    {
        // 같은 곳이면 무시
        if (from.owner == to.owner)
        {
            if (from.owner == SlotOwner.Inventory && from.invIndex == to.invIndex) return false;
            if (from.owner == SlotOwner.Equipment && from.equipSlot == to.equipSlot) return false;
        }

        // 인벤토리 ↔ 인벤토리
        if (from.owner == SlotOwner.Inventory && to.owner == SlotOwner.Inventory)
            return SwapInventory(from.invIndex, to.invIndex);

        // 인벤토리 → 장비 (장착)
        if (from.owner == SlotOwner.Inventory && to.owner == SlotOwner.Equipment)
            return EquipFromInventorySafe(from.invIndex, to.equipSlot);

        // 장비 → 인벤토리 (해제)
        if (from.owner == SlotOwner.Equipment && to.owner == SlotOwner.Inventory)
            return UnequipToInventory(from.equipSlot, to.invIndex);

        return false;
    }
    #endregion

    #region Private Function
    private bool SwapInventory(int a, int b)
    {
        if (!IsValidIndex(a) || !IsValidIndex(b)) return false;
        Swap(a, b); // 기존 메서드 사용 (동일 Countable이면 병합 처리 포함)
        return true;
    }

    public bool EquipFromInventorySafe(int invIndex, EquipmentSlotType targetSlot)
    {
        if (!IsValidIndex(invIndex)) return false;
        if (items[invIndex] is not EquipmentItem eq) return false;

        // 타입 일치 검증 (UI에서도 1차 검증하지만, 모델 레벨에서 재검증)
        if (eq.EquipmentData.slot != targetSlot) return false;

        // 타겟 슬롯에 이미 있으면 인벤토리 빈칸으로 이동
        if (equipped.TryGetValue(targetSlot, out var current) && current != null)
        {
            int empty = FindEmptySlotIndex();
            if (empty == -1) return false; // 빈칸 없으면 실패
            items[empty] = current;
            UpdateSlot(empty);
        }

        // 장착
        items[invIndex] = null;
        UpdateSlot(invIndex);
        equipped[targetSlot] = eq;
        OnEquippedChanged?.Invoke(targetSlot, eq);
        return true;
    }

    public bool UnequipToInventory(EquipmentSlotType fromSlot, int targetInvIndex)
    {
        if (!equipped.TryGetValue(fromSlot, out var src) || src == null) return false;
        if (!IsValidIndex(targetInvIndex)) return false;

        // 타겟칸이 비었으면 이동
        if (items[targetInvIndex] == null)
        {
            items[targetInvIndex] = src;
            UpdateSlot(targetInvIndex);
            equipped[fromSlot] = null;
            OnEquippedChanged?.Invoke(fromSlot, null);
            return true;
        }

        // 타겟칸이 차있으면 스왑 (프로젝트 정책에 따라 거부도 가능)
        var dst = items[targetInvIndex];
        items[targetInvIndex] = src;
        UpdateSlot(targetInvIndex);
        equipped[fromSlot] = dst as EquipmentItem; // 장비만 올 수 있음, 아니면 실패 처리해도 됨

        // 만약 dst가 장비가 아닐 수 있다면 → 거부하고 원복 로직으로 바꾸세요.
        OnEquippedChanged?.Invoke(fromSlot, equipped[fromSlot]);
        return true;
    }

    /// <summary>
    /// 장비 변경이 있을 때마다 호출해서 능력치를 재계산하고 적용한다.
    /// </summary>
    private void RecalculateStatsAndApply()
    {
        int atk = baseAttack;
        int def = baseDefense;
        int hp = baseMaxHP;
        float spd = baseMoveSpd;
        float drp = baseDropRate;

        foreach (var kv in equipped)
        {
            var eq = kv.Value;
            if (eq == null) continue;
            var data = (EquipmentItemData)eq.itemData;
            atk += data.attackValue;
            def += data.defenseValue;
            // 필요하면 장비에 이동속도/HP/드랍률 옵션 필드를 추가해서 같이 합산
        }

        // 능력치(힘/민첩/행운) 스케일을 포함해 최종치로 굳힌다.
        playerStatus.ApplyFinalStats(atk, def, hp, spd, drp);
    }

    private void ToggleEquip(int index, EquipmentItem eq)
    {
        var slot = eq.EquipmentData.slot;

        // 이미 같은 슬롯에 장비가 있으면 인벤토리로 반환(빈칸 필요)
        if (equipped.TryGetValue(slot, out var current))
        {
            int empty = FindEmptySlotIndex();
            if (empty == -1)
            {
                Debug.LogWarning("No empty slot to unequip current armor.");
                return;
            }
            items[empty] = current;
            UpdateSlot(empty);
        }

        // 현재 슬롯의 장비를 장착 목록으로 이동
        equipped[slot] = eq;
        items[index] = null;
        UpdateSlot(index);
        OnEquippedChanged?.Invoke(slot, eq);

        Debug.Log($"Equipped {eq.itemData.ItemName} to {slot}");
    }
    
    private void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = items[index];

        if(item is CountableItem ci && ci.IsEmpty)
        {
            items[index] = null;
            item = null;
        }

        OnSlotUpdated?.Invoke(index, item?.itemData);
    }

    private bool IsValidIndex(int index) => index >= 0 && index < Capacity;

    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < Capacity; i++)
            if (items[i] == null)
                return i;

        return -1; // 빈 슬롯 없음
    }

    private void RefreshAllSlots()
    {
        for (int i = 0; i < Capacity; i++) UpdateSlot(i);
    }

    private void RaiseGoldChanged() => OnGoldChanged?.Invoke(Gold);
    #endregion
}