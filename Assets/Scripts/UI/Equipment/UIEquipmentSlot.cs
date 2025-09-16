using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : UISlotBase, 
    IPointerClickHandler, IPointerMoveHandler
{
    [SerializeField] public EquipmentSlotType slotType;
    [SerializeField] Image borderImage;

    UIEquipment eqUI;
    private GameObject highLightGo;

    public override SlotAddress Address => SlotAddress.Eq(slotType);
    public override bool HasItem => inventory && inventory.GetEquippedItems().TryGetValue(slotType, out var item) && item != null;
    public override ItemData PeekItem()
    {
        if (!inventory) return null;
        return inventory.GetEquippedItems().TryGetValue(slotType, out var item) ? item?.itemData : null;
    }
    public override Sprite GetIcon() => iconImage ? iconImage.sprite : null;
    public override bool CanAccept(ItemData data)
    {
        if (data is EquipmentItemData eqData) return eqData.slot == slotType;
        return false;
    }
    public override void SetInventoryController(InventoryController ic) => inventory = ic;
    private void ShowHighLight() => highLightGo.SetActive(true);
    private void HideHighLight() => highLightGo.SetActive(false);

    protected override void Awake()
    {
        base.Awake();
        eqUI = GetComponentInParent<UIEquipment>();
        highLightGo = borderImage?.gameObject;
        HideHighLight();
    }

    void OnDisable()
    {
        TooltipService.I?.Hide();
        HideHighLight();
    }

    public void SetIcon(Sprite icon)
    {
        if (iconImage == null) return;
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    public void Clear() => SetIcon(null);

    public override void OnPointerEnter(PointerEventData eventData) => ShowHighLight();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        
        var eqUI = GetComponentInParent<UIEquipment>();
        if(eqUI) eqUI.RequestUnequip(slotType);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!HasItem || eqUI == null) { TooltipService.I?.Hide(); return; }
        if (!eqUI.TryGetEquipped(slotType, out var equipped)) { TooltipService.I?.Hide(); return; }

        var data = equipped.itemData;
        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            TooltipService.I?.Show(data.ItemName, data.Tooltip, data.Price, eventData.position,
                                   eqVal, !eqData.isArmor, eqData.isArmor);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        TooltipService.I?.Hide();
        HideHighLight();
    }
}
