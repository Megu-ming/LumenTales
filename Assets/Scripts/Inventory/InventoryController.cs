using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] UIInventory inventoryUI;

    public int Capacity { get; private set; }
    [SerializeField, Range(10, 30)] int initialCapacity = 30;
    [SerializeField, Range(0, 30)] int maxInventorySize = 30;

    // 실제 아이템 데이터
    private Item[] items;

    private void Awake()
    {
        items = new Item[maxInventorySize];
        Capacity = initialCapacity;
        if (inventoryUI != null) inventoryUI.SetInventoryReference(this);
        RefreshAllSlots();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var world = collision.GetComponent<WorldItem>();
        if (!world || world.itemData == null) return;

        int remainder = Add(world.itemData, world.amount);

        if(remainder<=0)
            world.CollectItem(transform);
        else world.amount = remainder;
    }

    #region public API
    public bool HasItem(int index) => IsValidIndex(index) && items[index] != null;
    public bool IsCountableItem(int index) => HasItem(index) && items[index] is CountableItem;

    public ItemData GetItemData(int index) => HasItem(index) ? items[index].itemData : null;
    public string GetItemName(int index) => GetItemData(index) ? GetItemData(index).ItemName : string.Empty;

    public int GetCurrentAmount(int index)
    {
        if (!IsCountableItem(index)) return 0;
        return ((CountableItem)items[index]).Amount;
    }
    #endregion

    public int Add(ItemData itemData, int amount = 1)
    {
        if (itemData == null || amount <= 0) return 0;

        if(itemData.IsStackable)
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
        inventoryUI.RemoveItem(index);
    }

    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

        Item itemA = items[indexA];
        Item itemB = items[indexB];

        // 셀 수 있고 동일한 아이템이면 A -> B로 개수 합치기
        if(itemA != null && itemB != null && 
            itemA.itemData == itemB.itemData && 
            itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            
        }
        else
        {
            items[indexA] = itemB;
            items[indexB] = itemA;
        }

        UpdateSlot(indexA); UpdateSlot(indexB);
    }

    public void OnInventoryToggle()
    {
        if (inventoryUI.gameObject.activeSelf)
            inventoryUI.Hide();
        else
            inventoryUI.Show();
    }

    #region Private Function
    private void UpdateSlot(int index)
    {
        if (!inventoryUI || !IsValidIndex(index)) return;

        Item item = items[index];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            // 아이콘 등록
            inventoryUI.SetItemIcon(index, item.itemData.Icon);

            // 1-1. 셀 수 있는 아이템
            if (item is CountableItem ci)
            {
                // 1-1-1. 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty)
                {
                    items[index] = null;
                    RemoveIcon();
                }
                
            }
            // 1-2. 셀 수 없는 아이템인 경우 수량 텍스트 제거
            else
            {
                
            }
        }
        // 2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            inventoryUI.RemoveItem(index);
        }
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
        if(!inventoryUI) return;
        for (int i = 0; i < Capacity; i++) UpdateSlot(i);
    }
    #endregion
}