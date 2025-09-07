using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UIInventory : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Options")]
    [SerializeField, ReadOnly] int inventoryCapacity;
    [SerializeField] UIInventoryItem slotPrefab;    // 아이템 슬롯 프리팹
    [SerializeField] RectTransform contentPanel;    // 스크롤뷰의 Content
    [SerializeField] GameObject ImageDummy;        // 드래그 중인 아이템 아이콘
    [SerializeField] 

    InventoryController inventory;

    [SerializeField] List<UIInventoryItem> slotUIList = new List<UIInventoryItem>();
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIInventoryItem beginDragSlot;      // 드래그를 시작한 슬롯
    private Transform beginDragIconTr;          // 해당 슬롯의 아이콘 트랜스폼

    private Vector3 beginDragIconPoint;         // 드래그 시작시 아이콘 위치
    private Vector3 beginDragCursorPoint;       // 드래그 시작시 커서 위치
    private int beginDragSlotIndex;          // 드래그 시작시 슬롯 인덱스

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Awake()
    {
        Init();
        InitSlot();
        Hide();
    }

    private void Update()
    {
        ped.position = Input.mousePosition;
    }

    private void Init()
    {
        TryGetComponent(out gr);
        if (gr == null)
            gr = gameObject.AddComponent<GraphicRaycaster>();
        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(10);

        inventoryCapacity = inventory.Capacity;
        // ToolTip UI

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
        if (slotUIList.Count == 0 || !slotUIList[index].HasItem) return;
        slotUIList[index].SetItemAmount(1);
    }

    public void RemoveItem(int index)
    {
        if (slotUIList.Count == 0 || !slotUIList[index].HasItem) return;
        slotUIList[index].RemoveItem();
    }

    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    /// <summary> 레이캐스트하여 얻은 첫 번째 UI에서 컴포넌트 찾아 리턴 </summary>
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

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == InputButton.Left) // 좌클릭
        {
            beginDragSlot = RaycastAndGetComponent<UIInventoryItem>();

            if(beginDragSlot != null && beginDragSlot.HasItem && beginDragSlot.IsAccessible)
            {
                beginDragSlotIndex = beginDragSlot.Index;
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
                if(ImageDummy) { ImageDummy.TryGetComponent<Image>(out Image img); img.enabled = false; }
                //beginDragIconTr.position = beginDragIconPoint;
                //beginDragSlot.transform.SetSiblingIndex(beginDragSlotIndex);

                beginDragSlot = null;
                beginDragIconTr = null;
            }
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
        // 버리기 구현
        else if(RaycastAndGetComponent<UIInventory>())
        {

        }

        // 드래그 시작 슬롯으로 복귀
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

    private void SetSlotIconInvisible(UIInventoryItem slot, bool visible)
    {
        if(slot?.itemImage != null)
            slot.itemImage.enabled = visible;
    }

    private void SetDummyFromSlot(UIInventoryItem slot, RectTransform rt)
    {
        var icon = slot?.itemImage;
        var dummy = ImageDummy?.GetComponent<Image>();
        dummy.sprite = icon?.sprite;
        var dummyRt = ImageDummy?.GetComponent<RectTransform>();
        dummyRt = rt;

        dummy.enabled = true;
    }

    private void SetDummyPosition(Vector3 pos)
    {
        if (ImageDummy != null)
            ImageDummy.transform.position = pos;
    }
}
