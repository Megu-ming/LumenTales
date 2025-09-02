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
