using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : UISlotBase, 
     IPointerMoveHandler
{
    [SerializeField] Image borderImage;
    [SerializeField] int index;

    private UIInventory inventoryUI;
    private GameObject highLightGo;

    public int Index => index;
    public override SlotAddress Address => SlotAddress.Inv(index);
    public override bool HasItem => inventory?.GetItemData(index) != null;
    public override ItemData PeekItem() => inventory?.GetItemData(index);
    public override Sprite GetIcon() => iconImage ? iconImage.sprite : null;

    public override bool CanAccept(ItemData data) => true;
    public override void SetInventoryController(InventoryController ic) => inventory = ic;

    protected override void Awake()
    {
        base.Awake();
        inventoryUI = GetComponentInParent<UIInventory>();
        highLightGo = borderImage.gameObject;
        iconImage.enabled = false;
        HideHighLight();
    }

    void OnDisable()
    {
        TooltipService.I?.Hide();
        HideHighLight();
    }

    public void SetSlotIndex(int index) => this.index = index;

    private void ShowHighLight() => highLightGo.SetActive(true);
    private void HideHighLight() => highLightGo.SetActive(false);

    public void SetItem(Sprite itemSprite)
    {
        if (!iconImage) return;
        iconImage.sprite = itemSprite;
        iconImage.enabled = itemSprite != null;
    }

    public void RemoveItem() => SetItem(null);

    public override void OnPointerEnter(PointerEventData eventData)
    {
        ShowHighLight();
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        // ToolTip UI
        if (!HasItem) { TooltipService.I?.Hide(); return; }
        var data = inventory.GetItemData(Index);

        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            TooltipService.I?.Show
                (data.ItemName, data.Tooltip, data.Price, eventData.position, eqVal, !eqData.isArmor, eqData.isArmor);
        }
        else
            TooltipService.I?.Show(data.ItemName, data.Tooltip, data.Price, eventData.position);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        TooltipService.I?.Hide();
        HideHighLight();
    }
}
