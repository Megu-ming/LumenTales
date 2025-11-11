using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler,IPointerExitHandler
{
    UIRoot uiRoot;
    InventoryController inven;

    public Image itemImage;
    [SerializeField] Image borderImage;
    [SerializeField] TextMeshProUGUI amountText;

    public int Index { get; private set; }
    public bool HasItem => itemImage.sprite != null;
    public RectTransform SlotRect => slotRect;
    public RectTransform IconRect => iconRect;

    private RectTransform slotRect;
    private RectTransform iconRect;

    private GameObject iconGo;
    private GameObject highLightGo;

    public void ShowIcon() => iconGo.SetActive(true);
    public void HideIcon() => iconGo.SetActive(false);

    private void ShowHighLight() => highLightGo.SetActive(true);
    private void HideHighLight() => highLightGo.SetActive(false);

    public void SetSlotIndex(int index) =>Index = index;

    public void Init(UIRoot uiRoot, InventoryController inven)
    {
        this.uiRoot = uiRoot;
        this.inven = inven;

        InitComponent();
        HideIcon();
        HideHighLight();
    }

    private void OnDisable()
    {
        HideHighLight();
    }

    private void InitComponent()
    {
        slotRect = GetComponent<RectTransform>();
        iconRect = itemImage.GetComponent<RectTransform>();
        iconGo = itemImage.gameObject;
        highLightGo = borderImage.gameObject;
    }

    public void SwapOrMoveIcon(UIInventorySlot other)
    {
        if(other == null) return;
        if(other == this) return;

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

    public void SetAmount(int amount)
    {
        if (amount > 1) amountText.text = amount.ToString();
        else amountText.text = "";
    }

    public void RemoveItem()
    {
        itemImage.sprite = null;
        HideIcon();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        ShowHighLight();
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        // ToolTip UI
        if (!HasItem) { uiRoot.Hide(); return; }
        var data = inven.GetItemData(Index);
        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            uiRoot.Show
                (data.ItemName, data.Tooltip, data.Price, eventData.position, eqVal, !eqData.isArmor, eqData.isArmor);
        }
        else
            uiRoot.Show(data.ItemName, data.Tooltip, data.Price, eventData.position);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        uiRoot.Hide();
        HideHighLight();
    }
}