using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UIInventory : UIBase, IPointerDownHandler
{
    [Header("Options")]
    [SerializeField, ReadOnly] int inventoryCapacity;
    [SerializeField] UIInventorySlot slotPrefab;    // 아이템 슬롯 프리팹
    [SerializeField] RectTransform contentPanel;    // 스크롤뷰의 Content
    [SerializeField] TextMeshProUGUI goldText;      // 골드 텍스트

    [SerializeField] InventoryController inventory;

    List<UIInventorySlot> slotUIList = new List<UIInventorySlot>();


    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (inventory == null) return;
        Init();
        InitSlot();
        UpdateGoldText(inventory.Gold);
        inventory.OnGoldChanged += UpdateGoldText;
        inventory.OnSlotUpdated += HandleSlotUpdated;

        for(int i = 0; i < inventory.Capacity; i++)
            HandleSlotUpdated(i, inventory.GetItemData(i));
    }

    private void OnDestroy()
    {
        if (inventory != null)
        { 
            inventory.OnGoldChanged -= UpdateGoldText;
            inventory.OnSlotUpdated -= HandleSlotUpdated;
        }
    }

    protected override void OnOpen()
    {
    }

    protected override void OnClose()
    {
    }

    public void OnInventoryToggle()
    {
        Toggle();
    }

    public void SetItemIcon(int index, Sprite icon)
    {
        slotUIList[index].SetItem(icon);
    }

    public void RemoveItem(int index)
    {
        if (slotUIList.Count == 0 || !slotUIList[index].HasItem) return;
        slotUIList[index].RemoveItem();
    }

    private void HandleSlotUpdated(int index, ItemData data)
    {
        if (data != null) SetItemIcon(index, data.Icon);
        else RemoveItem(index);
    }

    #region Event System Handlers
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        // 우클릭 : 아이템 사용
        if(eventData.button == InputButton.Right)
        {
            // TODO : 아이템 사용
            eventData.selectedObject.TryGetComponent<UIInventorySlot>(out var slot);
            if(slot!=null&&slot.HasItem)
            {
                inventory.UseAt(slot.Index);
            }
        }

        UIStackManager.Instance.BringToFront(this);
    }
    #endregion

    #region Private Methods
    private void Init()
    {
        inventoryCapacity = inventory.Capacity;
    }

    private void InitSlot()
    {
        for (int i = 0; i < inventoryCapacity; i++)
        {
            int slotIndex = i;

            var slot = Instantiate(slotPrefab, contentPanel);
            slot.gameObject.SetActive(true);
            slot.name = $"Slot[{slotIndex}]";
            slot.SetSlotIndex(slotIndex);
            slot.SetInventoryController(inventory);
            slotUIList.Add(slot);
        }
    }

    private void UpdateGoldText(int gold)
    {
        if (goldText != null) goldText.text = gold.ToString();
    }
#endregion
}
