using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] UIInventoryItem itemPrefab;
    [SerializeField] RectTransform contentPanel;
    //[SerializeField] UIInvectoryDescription itemDescription;

    // ItemSlotPool
    List<UIInventoryItem> itemSlotPool = new List<UIInventoryItem>();

    // 데이터 소스
    Dictionary<string, int> playerItems;
    [SerializeField] List<ItemData> itemDB;

    // mapping slot<>item
    private readonly Dictionary<string, UIInventoryItem> itemSlotMap = new Dictionary<string, UIInventoryItem>();

    private void Awake()
    {
        Hide();
        //itemDescription.ResetDescription();
    }

    public void InitializeInventoryUI(int inventorysize)
    {
        EnsureSlotCount(inventorysize);
    }

    //public void SetDataSource(Dictionary<string, int> playerItemDict, Dictionary<string, ItemData> itemDatabase)
    //{
    //    playerItems = playerItemDict;
    //    itemDB = itemDatabase;
    //    RefreshUI();
    //}

    //public void RefreshUI()
    //{
    //    if(playerItems == null || itemDB == null)
    //    {
    //        Debug.LogError("PlayerItems or ItemDB is not assigned in UIInventory.");
    //        return;
    //    }

    //    // 보유 아이템 목록 작성
    //    var entries = new List<(string id, ItemData data, int count)>();
    //    foreach (var kvp in playerItems)
    //    {
    //        if(kvp.Value <= 0) continue;
    //        if(!itemDB.TryGetValue(kvp.Key, out var data) || data == null)
    //        {
    //            Debug.LogWarning($"Item ID {kvp.Key} not found in ItemDB.");
    //            continue;
    //        }
    //        entries.Add((kvp.Key, data, kvp.Value));
    //    }

    //    // 정렬(원하면)
    //    // TODO

    //    itemSlotMap.Clear();

    //    for(int i = 0; i < itemSlotPool.Count; i++)
    //    {
    //        var slot = itemSlotPool[i];

    //        if (i < entries.Count)
    //        {
    //            var entry = entries[i];
                
    //            slot.SetData(entry.data.icon, entry.count);
    //            slot.gameObject.SetActive(true);
    //            itemSlotMap[slot] = entry.id;
    //        }
    //        else
    //        {
    //            //slot.Clear
    //            //slot.gameObject.SetActive(false);
    //        }
    //    }

    //    if(itemDescription != null)
    //        itemDescription.ResetDescription();
    //}

    private void EnsureSlotCount(int need)
    {
        while(itemSlotPool.Count<need)
        {
            var newItem = Instantiate(itemPrefab, contentPanel);
            itemSlotPool.Add(newItem);

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
        //itemDescription.ResetDescription();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
