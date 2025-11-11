using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StoreSlotUI : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    [SerializeField] StoreUI store;
    public Image borderImage;
    [SerializeField] Image itemImage;
    [SerializeField] TMPro.TMP_Text itemNameText;
    [SerializeField] TMPro.TMP_Text itemPriceText;

    private ItemData item;
    InventoryController inven;

    // 더블클릭 감지
    private float lastClickTime;
    private const float doubleClickTime = 0.2f; // Time in seconds

    public void Init(StoreUI store, InventoryController inven)
    {
        this.store = store;
        this.inven = inven;
    }

    public void SetItem(ItemData data)
    {
        if(data == null)
        {
            Clear();
            return;
        }
        item = data;
        itemImage.sprite = data.Icon;
        itemNameText.text = data.ItemName;
        itemPriceText.text = data.Price.ToString();
    }

    public void Clear()
    {
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemNameText.text = "";
        itemPriceText.text = "";
    }

    //EventSystems
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if(store.GetChoicedSlot() != this)
            store.ChoiceSlot(this);
        borderImage.color = Color.blue;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (Time.realtimeSinceStartup - lastClickTime < doubleClickTime)
        {
            // Call your double-click specific function here
            store.OpenModal();
        }
        lastClickTime = Time.realtimeSinceStartup;
    }

    public void TryBuyItem(bool value, int amount)
    {
        if (value is false) return;

        inven.BuyItem(item, amount);
    }
}
