using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;

    [SerializeField] TMP_Text quantityText;

    [SerializeField] Image borderImage;

    private UIInventory inventoryUI;
    public int Index { get; private set; }
    public bool HasItem => itemImage.sprite != null;
    public bool IsAccessible => isAccessibleSlot && isAccessibleItem;
    public RectTransform SlotRect => slotRect;
    public RectTransform IconRect => iconRect;

    private RectTransform slotRect;
    private RectTransform iconRect;

    private GameObject iconGo;
    private GameObject textGo;
    private GameObject highLightGo;

    private Image slotImage;

    private bool isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool isAccessibleItem = true; // 아이템 접근가능 여부

    public void ShowIcon() => iconGo.SetActive(true);
    public void HideIcon() => iconGo.SetActive(false);
    public void ShowText() => textGo.SetActive(true);
    public void HideText() => textGo.SetActive(false);

    private void ShowHighLight() => highLightGo.SetActive(true);
    private void HideHighLight() => highLightGo.SetActive(false);

    public void SetSlotIndex(int index) => Index = index;

    public void Awake()
    {
        InitComponent();
        HideIcon();
        HideText();
        HideHighLight();
    }

    private void InitComponent()
    {
        inventoryUI = GetComponentInParent<UIInventory>();

        slotRect = GetComponent<RectTransform>();
        iconRect = itemImage.GetComponent<RectTransform>();
        iconGo = itemImage.gameObject;
        textGo = quantityText.gameObject.transform.parent.gameObject;
        highLightGo = borderImage.gameObject;
        slotImage = GetComponent<Image>();
    }

    public void SetSlotAccessibleState(bool value)
    {
        if (isAccessibleSlot == value) return;

        if(value)
        {
            slotImage.color = Color.black;
        }
        else 
        {
            HideIcon();
            HideText();
        }

        isAccessibleSlot = value;
    }

    public void SetItemAccessibleState(bool value)
    {
        if(isAccessibleItem == value) return;

        if (value)
        {
            itemImage.color = Color.white;
            quantityText.color = Color.white;
        }

        isAccessibleItem = value;
    }

    public void SwapOrMoveIcon(UIInventoryItem other)
    {
        if(other == null) return;
        if(other == this) return;
        if(!this.IsAccessible) return;
        if (!other.IsAccessible) return;

        var temp = itemImage.sprite;

        if (other.HasItem) SetItem(other.itemImage.sprite);
        else RemoveItem();

        other.SetItem(temp);
    }

    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            itemImage.sprite = itemSprite;
            ShowIcon();
        }
        else RemoveItem();
    }

    public void RemoveItem()
    {
        itemImage.sprite = null;
        HideIcon();
        HideText();
    }

    public void SetItemAmount(int amount)
    {
        if(HasItem&&amount>1) ShowText();
        else HideText();

        quantityText.text = amount.ToString();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //if (!HasItem) return;
        if (!IsAccessible) return;
        ShowHighLight();
        //inventoryUI.ShowItemTooltip(Index, transform as RectTransform);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //if (!HasItem) return;
        if (!IsAccessible) return;
        HideHighLight();
        //inventoryUI.HideItemTooltip();
    }
}
