using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] int inventoryCapacity;
    [SerializeField] UIInventoryItem slotPrefab;    // ������ ���� ������
    [SerializeField] RectTransform contentPanel;    // ��ũ�Ѻ��� Content

    InventoryController inventory;

    [SerializeField] List<UIInventoryItem> slotUIList = new List<UIInventoryItem>();
    private GraphicRaycaster gr;
    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIInventoryItem pointerOverSlot;    // ���� �����Ͱ� ��ġ�� ����
    private UIInventoryItem beginDragSlot;      // �巡�׸� ������ ����
    private Transform beginDragIconTr;          // �ش� ������ ������ Ʈ������

    private Vector3 beginDragIconPoint;         // �巡�� ���۽� ������ ��ġ
    private Vector3 beginDragCursorPoint;       // �巡�� ���۽� Ŀ�� ��ġ
    private int beginDragSlotIndex;          // �巡�� ���۽� ���� �ε���

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

        OnPointerEnterAndExit();
        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
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

    /// <summary> ����ĳ��Ʈ�Ͽ� ���� ù ��° UI���� ������Ʈ ã�� ���� </summary>
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

    private void OnPointerEnterAndExit()
    {
        // �� �������� ����
        var prevSlot = pointerOverSlot;

        // ���� �������� ����
        var curSlot = pointerOverSlot = RaycastAndGetComponent<UIInventoryItem>();

        if(prevSlot == null)
        {
            if (curSlot != null)
                OnCurrentEnter();
        }
        else
        {
            if (curSlot == null)
                OnPrevExit();
            else if(prevSlot!=curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        void OnCurrentEnter()
        {
            // ���� �̹��� Ȱ��ȭ
            //if (_showHighlight)
            //    curSlot.Highlight(true);
        }
        void OnPrevExit()
        {
            // ���� �̹��� ��Ȱ��ȭ
            // prevSlot.Highlight(false);
        }
    }

    private void OnPointerDown()
    {
        if(Input.GetMouseButtonDown(0)) // ��Ŭ��
        {
            beginDragSlot = RaycastAndGetComponent<UIInventoryItem>();

            if(beginDragSlot != null && beginDragSlot.HasItem && beginDragSlot.IsAccessible)
            {
                beginDragIconTr = beginDragSlot.IconRect;
                beginDragIconPoint = beginDragIconTr.position;
                beginDragCursorPoint = Input.mousePosition;

                beginDragSlotIndex = beginDragSlot.Index;
                // �������� �ֻ�����
                beginDragIconTr.SetAsLastSibling();
            }
        }
        // ��Ŭ�� : ������ ���
    }

    private void OnPointerDrag()
    {
        if (beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            if(beginDragIconTr)
                beginDragIconTr.position = beginDragIconPoint + (Input.mousePosition - beginDragCursorPoint);
        }
    }

    private void OnPointerUp()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(beginDragSlot != null && beginDragIconTr != null)
            {
                beginDragIconTr.position = beginDragIconPoint;
                beginDragSlot.transform.SetSiblingIndex(beginDragSlotIndex);
                EndDrag();

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
}
