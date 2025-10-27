using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour, 
    IPointerEnterHandler,IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
{
    UIManager uiManager;
    UIEquipment eqUI => GetComponentInParent<UIEquipment>();
    public EquipmentSlotType slotType;
    
    public Image iconImage;
    [SerializeField] Image borderImage;

    public RectTransform IconRect => iconImage?.GetComponent<RectTransform>();
    public Sprite CurrentIcon => iconImage?.sprite;

    private GameObject highLightGo;
    private void ShowHighLight() => highLightGo.SetActive(true);
    private void HideHighLight() => highLightGo.SetActive(false);

    public void Init(UIManager uiManager)
    {
        this.uiManager = uiManager;

        highLightGo = borderImage.gameObject;
        HideHighLight();
    }

    void OnDisable()
    {
        HideHighLight();
    }

    public bool HasItem => iconImage != null && iconImage.sprite != null;

    public void SetIcon(Sprite icon)
    {
        if (iconImage == null) return;
        iconImage.sprite = icon;
        iconImage.enabled = icon != null;
    }

    public void Clear() => SetIcon(null);

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowHighLight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        
        var eqUI = GetComponentInParent<UIEquipment>();
        if(eqUI) eqUI.RequestUnequip(slotType);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(!HasItem) return;

        TryShowTooltip(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.Hide();
        HideHighLight();
    }

    void TryShowTooltip(Vector2 screenPos)
    {
        if (!HasItem) { uiManager.Hide(); return; }
        if (!eqUI) { uiManager.Hide(); return; }
        if(!eqUI.TryGetEquipped(slotType, out var equipped)) { uiManager.Hide(); return; }

        var data = equipped.itemData;
        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            uiManager.Show
                (data.ItemName, data.Tooltip, data.Price, screenPos, eqVal, !eqData.isArmor, eqData.isArmor);
        }
        else { uiManager.Hide(); return; }
    }
}
