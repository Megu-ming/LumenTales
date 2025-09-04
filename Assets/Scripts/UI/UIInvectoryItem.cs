using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] Image itemImage;

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

    private Image slotImage;

    private bool isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool isAccessibleItem = true; // 아이템 접근가능 여부

    /// <summary> 비활성화된 슬롯의 색상 </summary>
    private static readonly Color InAccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    /// <summary> 비활성화된 아이콘 색상 </summary>
    private static readonly Color InAccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void ShowIcon() => iconGo.SetActive(true);
    private void HideIcon() => iconGo.SetActive(false);

    private void ShowText() => textGo.SetActive(true);
    private void HideText() => textGo.SetActive(false);

    public void SetSlotIndex(int index) => Index = index;

    public void Awake()
    {
        InitComponent();
        HideIcon();
        HideText();
    }

    private void InitComponent()
    {
        inventoryUI = GetComponentInParent<UIInventory>();

        slotRect = GetComponent<RectTransform>();
        iconRect = itemImage.GetComponent<RectTransform>();
        iconGo = itemImage.gameObject;
        textGo = quantityText.gameObject;
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
            slotImage.color = InAccessibleSlotColor;
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
        else
        {
            itemImage.color = InAccessibleIconColor;
            quantityText.color = InAccessibleIconColor;
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
}
