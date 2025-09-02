using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] UIInventoryItem itemPrefab;
    [SerializeField] RectTransform contentPanel;
    [SerializeField] UIInvectoryDescription itemDescription;

    List<UIInventoryItem> itemList = new List<UIInventoryItem>();
    private void Awake()
    {
        Hide();
        itemDescription.ResetDescription();
    }

    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem newItem = Instantiate(itemPrefab, contentPanel);
            itemList.Add(newItem);
            newItem.OnItemClicked += HandleItemSelection;
            newItem.OnItemBeginDrag += HandleBeginDrag;
            newItem.OnItemEndDrag += HandleEndDrag;
            newItem.OnItemDroppedOn += HandleSwap;
            newItem.OnRightMouseBtnClicked += HandleShowItemActions;
        }
    }

    public bool IntoInventory(Item item, GameObject owner)
    {
        if (owner == null) return false;
        if (itemList == null) return false;

        // 아이템을 가지고 있는 지 찾기
        foreach(var uiItem in itemList)
        {
            if (uiItem.IsEmpty)
                continue;
            else
            {
                uiItem.SetData(item.Sprite, 1);
                return true;
            }
        }

        // 맨 앞에 새로 얻은 아이템 두기
        foreach (var uiItem in itemList)
        {
            if(uiItem.IsEmpty)
            {
                uiItem.SetData(item.Sprite, 1);
                return true;
            }
        }

        return false;
    }

    private void HandleItemSelection(UIInventoryItem item)
    {
        Debug.Log(item.name);
    }

    private void HandleBeginDrag(UIInventoryItem item)
    {
        
    }

    private void HandleEndDrag(UIInventoryItem item)
    {
        
    }

    private void HandleSwap(UIInventoryItem item)
    {
        
    }

    private void HandleShowItemActions(UIInventoryItem item)
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        itemDescription.ResetDescription();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
