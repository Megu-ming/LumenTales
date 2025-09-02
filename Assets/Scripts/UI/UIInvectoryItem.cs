using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] Image itemImage;
    public Sprite ItemSprite { get { return itemImage.sprite; } }

    [SerializeField] TMP_Text quantityText;
    public TMP_Text QuantityText { get {return quantityText;} }
    public int quantity = 0;

    [SerializeField] Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked, 
        OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, 
        OnRightMouseBtnClicked;

    private bool empty = true;
    public bool IsEmpty { get { return empty; } }

    public void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
        empty = true;
    }

    public void Deselect()
    {
        borderImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        quantityText.text = (this.quantity + quantity).ToString();
        empty = false;
    }

    public void Select()
    {
        borderImage.enabled = true;
    }

    public void OnBegindDrag()
    {
        if (empty)
            return;
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop()
    {
        OnItemDroppedOn?.Invoke(this);
    }

    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(BaseEventData data)
    {
        if (empty)
            return;
        PointerEventData pointerData = (PointerEventData)data;
        if(pointerData.button == PointerEventData.InputButton.Right)
            OnRightMouseBtnClicked?.Invoke(this);
        else
            OnItemClicked?.Invoke(this);

    }
}
