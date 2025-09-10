using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour
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
}
