using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] UIInventoryItem itemPrefab;
    [SerializeField] RectTransform contentPanel;

    List<UIInventoryItem> itemList = new List<UIInventoryItem>();

    public void InitializeInventoryUI(int inventorysize)
    {
        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem newItem = Instantiate(itemPrefab, contentPanel);
            itemList.Add(newItem);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
