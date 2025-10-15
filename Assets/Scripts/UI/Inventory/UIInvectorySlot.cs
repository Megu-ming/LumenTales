using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler,IPointerExitHandler
{
    public Image itemImage;
    [SerializeField] Image borderImage;
    [SerializeField] TextMeshProUGUI amountText;

    private UIInventory inventoryUI;
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

    public void Awake()
    {
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
        inventoryUI = UIManager.instance.inventoryUI;

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
        if (!HasItem) { UIManager.instance?.Hide(); return; }
        if (inventoryUI == null) { UIManager.instance?.Hide(); return; }
        var data = Player.instance?.InventoryController.GetItemData(Index);
        if (data is EquipmentItemData eqData)
        {
            int eqVal = eqData.isArmor ? eqData.defenseValue : eqData.attackValue;
            UIManager.instance?.Show
                (data.ItemName, data.Tooltip, data.Price, eventData.position, eqVal, !eqData.isArmor, eqData.isArmor);
        }
        else
            UIManager.instance?.Show(data.ItemName, data.Tooltip, data.Price, eventData.position);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance?.Hide();
        HideHighLight();
    }
}