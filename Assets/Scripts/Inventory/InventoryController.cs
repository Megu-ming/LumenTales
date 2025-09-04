using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("InventoryUISetting")]
    [SerializeField] UIInventory inventoryUI;

    public int Capacity { get; private set; }
    [SerializeField, Range(10, 30)] int initialCapacity = 10;
    [SerializeField, Range(0, 30)] int maxInventorySize = 30;


    // 실제 아이템 데이터
    [SerializeField] private Item[] items;

    [SerializeField] private int goldAmount = 0;
    public int GoldAmount { get { return goldAmount; } set { goldAmount = value; } }

    private void Awake()
    {
        items = new Item[maxInventorySize];
        Capacity = initialCapacity;
        inventoryUI.SetInventoryReference(this);
    }

    private void Start()
    {
        UpdateAccessibleStatesAll();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.gameObject.GetComponent<Item>();
        if (item!=null)
        {
            // 가방이 꽉 찼다면 false로 아이템을 수집하지 않음
            int emptyIndex = FindEmptySlotIndex();
            if (emptyIndex != -1)
            {
                item.CollectItem(transform);
                items[emptyIndex] = item;
                inventoryUI.SetItemIcon(emptyIndex, item.itemData.Icon);
                if (item is CountableItem ci)
                    inventoryUI.SetItemAmountText(emptyIndex, ci.Amount);
                else
                    inventoryUI.HideItemAmountText(emptyIndex);
            }
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < Capacity;
    }

    private int FindEmptySlotIndex(int startIndex=0)
    {
        for(int i = startIndex; i < Capacity; i++)
            if (items[i] == null)
                return i;
        
        return -1; // 빈 슬롯 없음
    }

    public void UpdateAccessibleStatesAll()
    {
        inventoryUI.SetAccessibleSlotRange(Capacity);
    }

    public bool HasItem(int index)
    {
        return IsValidIndex(index) && items[index] != null;
    }

    public bool IsCountableItem(int index)
    {
        return HasItem(index) && items[index] is CountableItem;
    }

    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (items[index] == null) return 0;

        CountableItem ci = items[index] as CountableItem;
        if (ci == null)
            return 1;

        return ci.Amount;
    }

    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index) || items[index] == null) return null;
        return items[index].itemData;
    }

    public string GetItemName(int index)
    {
        if (!IsValidIndex(index) || items[index] == null) return null;
        return items[index].itemData.ItemName;
    }

    public void OnInventoryToggle()
    {
        if (inventoryUI.gameObject.activeSelf)
            inventoryUI.Hide();
        else
            inventoryUI.Show();
    }
}