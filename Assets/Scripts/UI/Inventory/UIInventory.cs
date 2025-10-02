using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;
using static Utils;

public class UIInventory : UIBase
{
    [Header("Options")]
    [SerializeField, ReadOnly] int inventoryCapacity;
    [SerializeField] UIInventorySlot slotPrefab;    // 아이템 슬롯 프리팹
    [SerializeField] RectTransform contentPanel;    // 스크롤뷰의 Content
    [SerializeField] TextMeshProUGUI goldText;      // 골드 텍스트

    public InventoryController inventory;

    List<UIInventorySlot> slotUIList = new List<UIInventorySlot>();
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIInventorySlot beginDragSlot;      // 드래그를 시작한 슬롯
    private Transform beginDragIconTr;          // 해당 슬롯의 아이콘 트랜스폼

    private Vector3 beginDragIconPoint;         // 드래그 시작시 아이콘 위치
    private Vector3 beginDragCursorPoint;       // 드래그 시작시 커서 위치

    public bool isAttachedToStore = false;

    public void Init()
    {
        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);

        inventoryCapacity = inventory.Capacity;

        for (int i = 0; i < inventoryCapacity; i++)
        {
            int slotIndex = i;

            var slot = Instantiate(slotPrefab, contentPanel);
            slot.gameObject.SetActive(true);
            slot.name = $"Slot[{slotIndex}]";
            slot.SetSlotIndex(slotIndex);
            slotUIList.Add(slot);
        }

        UpdateGoldText(inventory.Gold);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (inventory == null) inventory = Player.instance?.InventoryController;
        if (inventory == null) return;

        Init();

        inventory.OnGoldChanged += UpdateGoldText;
        inventory.OnSlotUpdated += HandleSlotUpdated;
        inventory.OnSlotTextUpdated += HandleSlotTextUpdated;
        inventory.OnInvenUIToggleRequest += Toggle;

        for (int i = 0; i < inventory.Capacity; i++)
            HandleSlotUpdated(i, inventory.GetItemData(i));
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(ped != null) ped.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        if (inventory != null)
        { 
            inventory.OnGoldChanged -= UpdateGoldText;
            inventory.OnSlotUpdated -= HandleSlotUpdated;
            inventory.OnSlotTextUpdated -= HandleSlotTextUpdated;
            inventory.OnInvenUIToggleRequest -= Toggle;
        }
    }

    protected override void OnOpen()
    {
        if (inventory is not null)
            inventory.RefreshAllSlots();
    }

    protected override void OnClose()
    {
        
    }

    public void SetItemIcon(int index, Sprite icon)
    {
        slotUIList[index].SetItem(icon);
    }

    public void SetItemAmount(int index, int amount)
    {
        slotUIList[index].SetAmount(amount);
    }

    public void RemoveItem(int index)
    {
        if (slotUIList.Count == 0 || !slotUIList[index].HasItem) return;
        slotUIList[index].RemoveItem();
    }

    private void HandleSlotUpdated(int index, ItemData data)
    {
        if (data != null)
        { 
            SetItemIcon(index, data.Icon); 
        }
        else RemoveItem(index);
    }

    private void HandleSlotTextUpdated(int index, int amount)
    {
        SetItemAmount(index, amount);
    }

    #region Event System Handlers
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (eventData.button == InputButton.Left) // 좌클릭
        {
            beginDragSlot = RaycastAndGetComponent<UIInventorySlot>(rrList, ped);

            if(beginDragSlot != null && beginDragSlot.HasItem)
            {
                beginDragIconTr = beginDragSlot.IconRect;
                beginDragIconPoint = beginDragIconTr.position;
                beginDragCursorPoint = Input.mousePosition;

                SetSlotIconInvisible(beginDragSlot, false);

                SetDummyFromSlot(beginDragSlot, beginDragSlot.IconRect);
                SetDummyPosition(beginDragIconPoint);
            }
        }
        // 우클릭 : 아이템 사용
        else if(eventData.button == InputButton.Right)
        {
            // TODO : 아이템 사용
            var slot = RaycastAndGetComponent<UIInventorySlot>(rrList, ped);
            if(slot!=null&&slot.HasItem)
            {
                inventory.UseAt(slot.Index);
            }
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (beginDragSlot == null) return;
        UIManager.instance.BringToFront(this);
        if (eventData.button == InputButton.Left)
        {
            if (beginDragIconTr)
            {
                Vector3 pos = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
                SetDummyPosition(pos);
            }
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (eventData.button == InputButton.Left)
        {
            if(beginDragSlot != null && beginDragIconTr != null)
            {
                EndDrag();

                SetSlotIconInvisible(beginDragSlot, true);
                if (UIManager.instance.dummy) { UIManager.instance.dummy.TryGetComponent<Image>(out Image img); img.enabled = false; }

                beginDragSlot = null;
                beginDragIconTr = null;
            }
        }
    }
    #endregion

    #region Private Methods
    int sellingIndex;
    private void EndDrag()
    {
        var endDragSlot = RaycastAndGetComponent<UIInventorySlot>(rrList, ped);

        if(endDragSlot != null)
        {
            bool isSeparatable =
                   (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                   (inventory.IsCountableItem(beginDragSlot.Index) && !inventory.HasItem(endDragSlot.Index));

            bool isSeparation = false;
            int currentAmount = 0;

            // 현재 개수 확인
            if(isSeparatable)
            {
                currentAmount = inventory.GetCurrentAmount(beginDragSlot.Index);
                if (currentAmount > 1) isSeparation = true;
            }

            if (isSeparation)
                TrySeparateAmount(beginDragSlot.Index, endDragSlot.Index, currentAmount);
            else TrySwapItems(beginDragSlot, endDragSlot);

            return;
        }

        // 장비창 슬롯 위 드롭
        var eqSlot = RaycastAndGetComponent<UIEquipmentSlot>(rrList, ped);
        if(eqSlot != null) // 장비타입에 맞는 창인지도 확인해야함
        {
            inventory.EquipFromInventory(beginDragSlot.Index, eqSlot.slotType);
            return;
        }

        // 상점 UI위 드롭하면 판매
        var store = RaycastAndGetComponent<StoreSlotUI>(rrList, ped);
        if(store != null)
        {
            // 모달 열기
            var modal = UIManager.instance.inputFieldModal;
            if(inventory.GetItemData(beginDragSlot.Index) is CountableItemData)
            {
                int amount = inventory.GetCurrentAmount(beginDragSlot.Index);
                modal.maxAmount = amount;
            }
            else
            {
                modal.maxAmount = 1;
            }
            modal.Open();

            sellingIndex = beginDragSlot.Index;
            return;
        }
    }

    private void TrySwapItems(UIInventorySlot from,  UIInventorySlot to)
    {
        if (from == to)
            return;

        from.SwapOrMoveIcon(to);
        inventory.Swap(from.Index, to.Index);
    }

    public void TrySellItem(bool value, int amount)
    {
        if (value is true)
            inventory.SellItem(sellingIndex);
    }

    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if (indexA == indexB) return;

        string itemName = $"{inventory.GetItemName(indexA)} x{amount}";
    }

    private void UpdateGoldText(int gold)
    {
        if (goldText != null) goldText.text = gold.ToString();
    }

    private void SetSlotIconInvisible(UIInventorySlot slot, bool visible)
    {
        if (slot?.itemImage != null)
            slot.itemImage.enabled = visible;
    }

    private void SetDummyFromSlot(UIInventorySlot slot, RectTransform rt)
    {
        var icon = slot?.itemImage;
        var dummy = UIManager.instance.dummy?.GetComponent<Image>();
        dummy.sprite = icon?.sprite;
        var dummyRt = UIManager.instance.dummy?.GetComponent<RectTransform>();
        dummyRt = rt;

        dummy.enabled = true;
    }

    private void SetDummyPosition(Vector3 pos)
    {
        if (UIManager.instance.dummy != null)
        {
            UIManager.instance.dummy.transform.position = pos;
            UIManager.instance.dummy.transform.SetAsLastSibling();
        }
    }
#endregion
}
