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
    [SerializeField] UIInventoryItem slotPrefab;    // ������ ���� ������
    [SerializeField] RectTransform contentPanel;    // ��ũ�Ѻ��� Content
    [SerializeField] GameObject imageDummy;        // �巡�� ���� ������ ������
    [SerializeField] GameObject tooltipPrefab;      // ���� ������
    [SerializeField] TextMeshProUGUI goldText;      // ��� �ؽ�Ʈ

    InventoryController inventory;

    List<UIInventoryItem> slotUIList = new List<UIInventoryItem>();
    private UIItemTooltip tooltip;
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIInventoryItem beginDragSlot;      // �巡�׸� ������ ����
    private Transform beginDragIconTr;          // �ش� ������ ������ Ʈ������

    private Vector3 beginDragIconPoint;         // �巡�� ���۽� ������ ��ġ
    private Vector3 beginDragCursorPoint;       // �巡�� ���۽� Ŀ�� ��ġ

    public void Show() => gameObject.SetActive(true);
    public void Hide()
    {
        gameObject.SetActive(false);
        tooltip?.gameObject.SetActive(false);
    }

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        if(ped != null) ped.position = Input.mousePosition;
    }

    private void OnDestroy()
    {
        if (inventory != null) inventory.OnGoldChanged -= UpdateGoldText;
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

    public void SetInventoryReference(InventoryController inventory)
    {
        this.inventory = inventory;
        Init();
        InitSlot();
        UpdateGoldText(inventory.Gold);
        inventory.OnGoldChanged += UpdateGoldText;
    }

    private void UpdateGoldText(int gold)
    {
        if (goldText != null) goldText.text = gold.ToString();
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

    /// <summary> ����ĳ��Ʈ�Ͽ� ���� UI���� ������Ʈ ã�� ���� </summary>
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

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        // ToolTip UI
        UIInventoryItem slot = RaycastAndGetComponent<UIInventoryItem>();
        if (slot && slot.HasItem)
        {
            int index = slot.Index;
            var data = inventory.GetItemData(index);
            tooltip.SetupTooltip(data.ItemName, data.Tooltip, data.Price);
            tooltip.gameObject.SetActive(true);
            tooltip.transform.position = eventData.position;
        }
        else
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == InputButton.Left) // ��Ŭ��
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
        // ��Ŭ�� : ������ ���
        else if(eventData.button == InputButton.Right)
        {
            // TODO : ������ ���
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

            // ���� ���� Ȯ��
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
        // ������ ����
        else if(RaycastAndGetComponent<UIInventory>())
        {

        }

        // �巡�� ���� �������� ����
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
}
