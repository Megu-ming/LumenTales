using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [Header("Options")]
    [SerializeField, Range(0,10)] int horizontalSlotCount = 5;  // 슬롯 가로 개수
    [SerializeField, Range(0,10)] int verticalSlotCount = 6;    // 슬롯 세로 개수

    [SerializeField] UIInventoryItem slotPrefab;    // 아이템 슬롯 프리팹
    [SerializeField] RectTransform contentPanel;    // 스크롤뷰의 Content

    InventoryController inventory;

    [SerializeField] List<UIInventoryItem> slotUIList;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        InitSlot();
        Hide();
    }

    private void InitSlot()
    {
        slotUIList = new List<UIInventoryItem>(horizontalSlotCount * verticalSlotCount);

        for(int j=0;j < verticalSlotCount; j++)
        {
            for (int i = 0; i < horizontalSlotCount; i++)
            {
                int slotIndex = (horizontalSlotCount * j) + i;

                var slot = Instantiate(slotPrefab, contentPanel);
                slot.gameObject.SetActive(true);
                slot.name = $"Slot[{slotIndex}]";
                slot.SetSlotIndex(slotIndex);
                slotUIList.Add(slot);
            }
        }
    }

    public void SetInventoryReference(InventoryController inventory)
    {
        this.inventory = inventory;
    }

    public void SetItemIcon(int index, Sprite icon)
    {
        slotUIList[index].SetItem(icon);
    }

    public void SetItemAmountText(int index, int amount)
    {
        slotUIList[index].SetItemAmount(amount);
    }

    public void HideItemAmountText(int index)
    {
        slotUIList[index].SetItemAmount(1);
    }

    public void RemoveItem(int index)
    {
        slotUIList[index].RemoveItem();
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }
}
