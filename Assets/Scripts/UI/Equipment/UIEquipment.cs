using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.PointerEventData;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utils;

public class UIEquipment : UIBase
{
    [SerializeField] UIEquipmentSlot[] slots;
    [SerializeField] InventoryController inventory;

    [SerializeField] GameObject imageDummy;
    
    private readonly Dictionary<EquipmentSlotType, UIEquipmentSlot> map = new();

    private PointerEventData ped;
    private List<RaycastResult> rrList;

    private UIEquipmentSlot beginDragSlot;      // 드래그를 시작한 슬롯
    private Transform beginDragIconTr;          // 해당 슬롯의 아이콘 트랜스폼

    private Vector2 beginDragIconPoint;         // 드래그 시작시 아이콘 위치
    private Vector2 beginDragCursorPoint;       // 드래그 시작시 커서 위치

    public void Init()
    {
        map.Clear();
        foreach (var s in slots)
        {
            if (s == null) continue;
            map[s.slotType] = s;
            s.Clear();
        }

        inventory.OnEquippedChanged += HandleEquippedChanged;
        inventory.OnEquipUIToggleRequest += Toggle;

        foreach (var kv in inventory.GetEquippedItems())
        {
            if (map.TryGetValue(kv.Key, out var ui)) ui.SetIcon(kv.Value.itemData.Icon);
        }

        ped = new PointerEventData(EventSystem.current);
        rrList = new List<RaycastResult>(16);
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

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if(inventory != null) 
        {
            inventory.OnEquippedChanged -= HandleEquippedChanged;
            inventory.OnEquipUIToggleRequest -= Toggle;
        }
    }

    protected override void OnOpen()
    {
    }
    protected override void OnClose()
    {
    }

    private void HandleEquippedChanged(EquipmentSlotType slot, EquipmentItem itemOrNull)
    {
        if (!map.TryGetValue(slot, out var ui)) return;
        
        if(itemOrNull == null) ui.SetIcon(null);
        else ui.SetIcon(itemOrNull.itemData?.Icon);
    }

    public void RequestUnequip(EquipmentSlotType slot) => inventory?.Unequip(slot);

    public bool TryGetEquipped(EquipmentSlotType slot, out EquipmentItem item)
    {
        item = null;
        if (inventory == null) return false;
        var equipped = inventory.GetEquippedItems();
        if (!equipped.TryGetValue(slot, out item)) return false;
        return item != null;
    }

    #region Event System Handlers
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        
        if(eventData.button != InputButton.Left) return;

        ped.position = eventData.position;
        beginDragSlot = RaycastAndGetComponent<UIEquipmentSlot>(rrList, ped);
        if(beginDragSlot == null || !beginDragSlot.HasItem)
        {
            beginDragSlot = null;
            return;
        }

        if(imageDummy && imageDummy.TryGetComponent<Image>(out Image img))
        {
            img.sprite = beginDragSlot.CurrentIcon;
            img.enabled = true;
            SetSlotIconInvisible(beginDragSlot, false);
            imageDummy.transform.position = beginDragSlot.IconRect.position;
            imageDummy.transform.SetAsLastSibling();
        }

        beginDragIconPoint = beginDragSlot.IconRect.position;
        beginDragCursorPoint = eventData.position;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (beginDragSlot == null) return;
        if (eventData.button != InputButton.Left) return;

        if (imageDummy)
        {
            Vector3 pos = beginDragIconPoint + (eventData.position - beginDragCursorPoint);
            imageDummy.transform.position = pos;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (eventData.button != InputButton.Left) return;
        if (beginDragSlot != null)
        {
            ped.position = eventData.position;

            // 1) 인벤토리 슬롯으로 드롭 → 해제/이동
            var invSlot = RaycastAndGetComponent<UIInventorySlot>(rrList, ped);
            if (invSlot != null)
            {
                inventory.Unequip(beginDragSlot.slotType, invSlot.Index);

                SetSlotIconInvisible(beginDragSlot, false);
                EndDragCleanup();
                beginDragSlot = null;
                return;
            }

            // 2) 장비창 다른 슬롯으로는 교환 생략 (요구사항대로 아무 것도 안 함)

            // 3) UI 밖으로 드롭 → 나중에 아이템 버리기 로직
            // TODO: Drop equipped item into world when released outside UI.

            SetSlotIconInvisible(beginDragSlot, true);
            EndDragCleanup();
            beginDragSlot = null;
        }
    }

    private void EndDragCleanup()
    {
        if (imageDummy && imageDummy.TryGetComponent<Image>(out var img))
            img.enabled = false;
    }

    private void SetSlotIconInvisible(UIEquipmentSlot slot, bool val)
    {
        if (slot?.iconImage != null)
            slot.iconImage.enabled = val;
    }
    #endregion
}