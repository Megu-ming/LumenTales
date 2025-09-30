using UnityEngine;
using UnityEngine.UI;

public class StoreSlotUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TMPro.TMP_Text itemNameText;
    [SerializeField] TMPro.TMP_Text itemPriceText;

    public void SetItem(ItemData data)
    {
        if(data == null)
        {
            Clear();
            return;
        }

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
}
