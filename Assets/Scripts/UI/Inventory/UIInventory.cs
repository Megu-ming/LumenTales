using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UIInventory : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerMoveHandler
{
    [Header("Options")]
    [SerializeField, ReadOnly] int inventoryCapacity;
    [SerializeField] UIInventoryItem slotPrefab;    // 아이템 슬롯 프리팹
    [SerializeField] RectTransform contentPanel;    // 스크롤뷰의 Content
    [SerializeField] GameObject imageDummy;        // 드래그 중인 아이템 아이콘
    [SerializeField] GameObject tooltipPrefab;      // 툴팁 프리팹
    [SerializeField] TextMeshProUGUI goldText;      // 골드 텍스트

    [SerializeField] InventoryController inventory;

    List<UIInventoryItem> slotUIList = new List<UIInventoryItem>();
    private Canvas rootCanvas;
    private UIItemTooltip tooltip;
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIInventoryItem beginDragSlot;      // 드래그를 시작한 슬롯
    private Transform beginDragIconTr;          // 해당 슬롯의 아이콘 트랜스폼

    private Vector3 beginDragIconPoint;         // 드래그 시작시 아이콘 위치
    private Vector3 beginDragCursorPoint;       // 드래그 시작시 커서 위치

    public void Show() => gameObject.SetActive(true);
    public void Hide()
    {
        gameObject.SetActive(false);
        tooltip?.gameObject.SetActive(false);
    }

    private void Awake()
    {
        rootCanvas = gameObject.transform.parent.GetComponent<Canvas>();
        Hide();
    }

    private void Start()
    {
        if (inventory == null) return;
        Init();
        InitSlot();
        UpdateGoldText(inventory.Gold);
        inventory.OnGoldChanged += UpdateGoldText;
        inventory.OnSlotUpdated += HandleSlotUpdated;

        for (int i = 0; i < inventory.Capacity; i++)
            HandleSlotUpdated(i, inventory.GetItemData(i));
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
        }
    }

    public void OnInventoryToggle()
    {
        if (gameObject.activeSelf) Hide();
        else Show(); 
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
    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        // ToolTip UI
        UIInventoryItem slot = RaycastAndGetComponent<UIInventoryItem>();
        if (slot && slot.HasItem)
        {
            int index = slot.Index;
            var data = inventory.GetItemData(index);
            tooltip.SetupTooltip(data.ItemName, data.Tooltip, data.Price);
            tooltip.TryGetComponent<RectTransform>(out RectTransform tooltipRt);
            rootCanvas.TryGetComponent<RectTransform>(out RectTransform canvasRt);
            // 먼저 마우스 위치에 놓기
            tooltip.transform.position = eventData.position;

            // 화면 좌표 → 캔버스 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRt, eventData.position, rootCanvas.worldCamera, out Vector2 localPoint);

            // pivot 계산
            float pivotX = (localPoint.x + tooltipRt.sizeDelta.x > canvasRt.rect.width * 0.5f) ? 1f : 0f;
            float pivotY = (localPoint.y - tooltipRt.sizeDelta.y < -canvasRt.rect.height * 0.5f) ? 0f : 1f;

            tooltipRt.pivot = new Vector2(pivotX, pivotY);

            tooltip.gameObject.SetActive(true);
        }
        else
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == InputButton.Left) // 좌클릭
        {
            beginDragSlot = RaycastAndGetComponent<UIInventoryItem>();

            if(beginDragSlot != null && beginDragSlot.HasItem && beginDragSlot.IsAccessible)
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
            var slot = RaycastAndGetComponent<UIInventoryItem>();
            if(slot!=null&&slot.HasItem && slot.IsAccessible)
            {
                inventory.UseAt(slot.Index);
            }
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (beginDragSlot == null) return;

        if (eventData.button == InputButton.Left)
        {
            if (beginDragIconTr)
            {
                Vector3 pos = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
                SetDummyPosition(pos);
            }
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == InputButton.Left)
        {
            if(beginDragSlot != null && beginDragIconTr != null)
            {
                EndDrag();

                SetSlotIconInvisible(beginDragSlot, true);
                if (imageDummy) { imageDummy.TryGetComponent<Image>(out Image img); img.enabled = false; }
                //beginDragIconTr.position = beginDragIconPoint;
                //beginDragSlot.transform.SetSiblingIndex(beginDragSlotIndex);

                beginDragSlot = null;
                beginDragIconTr = null;
            }
        }
    }
    #endregion

    #region Private Methods
    private void Init()
    {
        TryGetComponent(out gr);
        if (gr == null)
            gr = gameObject.AddComponent<GraphicRaycaster>();
        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);

        inventoryCapacity = inventory.Capacity;
        // ToolTip UI
        tooltip = Instantiate(tooltipPrefab).GetComponent<UIItemTooltip>();
        tooltip.transform.SetParent(transform.parent);
        tooltip.gameObject.SetActive(false);
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
            slotUIList.Add(slot);
        }
    }

    private void EndDrag()
    {
        var endDragSlot = RaycastAndGetComponent<UIInventoryItem>();

        if(endDragSlot != null && endDragSlot.IsAccessible)
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
        var eqSlot = RaycastAndGetComponent<UIEquipmentSlot>();
        if(eqSlot!=null) // 장비타입에 맞는 창인지도 확인해야함
        {
            inventory.EquipFromInventory(beginDragSlot.Index, eqSlot.slotType);
            return;
        }
        
        // 버리기 구현
        // TODO

        // 드래그 시작 슬롯으로 복귀
    }

    /// <summary> 레이캐스트하여 얻은 UI에서 컴포넌트 찾아 리턴 </summary>
    private T RaycastAndGetComponent<T>() where T : Component
    {
        rrList.Clear();

        gr.Raycast(ped, rrList);

        if (rrList.Count == 0)
            return null;

        foreach (var rr in rrList)
        {
            var result = rr.gameObject.GetComponent<T>();
            if (result != null)
                return result;
        }

        return null;
    }

    private void TrySwapItems(UIInventoryItem from,  UIInventoryItem to)
    {
        if (from == to)
            return;

        from.SwapOrMoveIcon(to);
        inventory.Swap(from.Index, to.Index);
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

    private void SetSlotIconInvisible(UIInventoryItem slot, bool visible)
    {
        if (slot?.itemImage != null)
            slot.itemImage.enabled = visible;
    }

    private void SetDummyFromSlot(UIInventoryItem slot, RectTransform rt)
    {
        var icon = slot?.itemImage;
        var dummy = imageDummy?.GetComponent<Image>();
        dummy.sprite = icon?.sprite;
        var dummyRt = imageDummy?.GetComponent<RectTransform>();
        dummyRt = rt;

        dummy.enabled = true;
    }

    private void SetDummyPosition(Vector3 pos)
    {
        if (imageDummy != null)
            imageDummy.transform.position = pos;
    }
#endregion
}
