using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour, IPointerClickHandler
{
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
}
