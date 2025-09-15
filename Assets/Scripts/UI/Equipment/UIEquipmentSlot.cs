using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour, 
    IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
{
    UIEquipment eqUI => GetComponentInParent<UIEquipment>();
    public EquipmentSlotType slotType;
    [SerializeField] Image iconImage;

    private void Awake()
    {
        iconImage = GetComponent<Image>();
    }

    public bool HasItem => iconImage != null && iconImage.sprite != null;

    public void SetIcon(Sprite icon)
    {
        if (iconImage == null) return;
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    public void Clear() => SetIcon(null);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        
        var eqUI = GetComponentInParent<UIEquipment>();
        if(eqUI) eqUI.RequestUnequip(slotType);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(TooltipService.I == null) return;
        if(!HasItem) return;

        TryShowTooltip(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipService.I?.Hide();
    }

    void TryShowTooltip(Vector2 screenPos)
    {
        if (!HasItem) { TooltipService.I?.Hide(); return; }
        if (!eqUI) { TooltipService.I?.Hide(); return; }
        if(!eqUI.TryGetEquipped(slotType, out var equipped)) { TooltipService.I?.Hide(); return; }

        var data = equipped.itemData;
        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            TooltipService.I?.Show
                (data.ItemName, data.Tooltip, data.Price, screenPos, eqVal, !eqData.isArmor, eqData.isArmor);
        }
        else { TooltipService.I?.Hide(); return; }
    }
}
