using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("InventoryUISetting")]
    [SerializeField] UIInventory inventoryUI;
    [SerializeField] int inventorySize = 10;

    // 실제 아이템 데이터
    private Dictionary<string, int> items = new Dictionary<string, int>();
    public Dictionary<string, int> Items { get { return items; } }

    [SerializeField] private int goldAmount = 0;
    public int GoldAmount { get { return goldAmount; } set { goldAmount = value; } }

    private void Start()
    {
        if (inventoryUI == null)
            Debug.LogError("Inventory UI is not assigned in InventoryController.");
        else
            inventoryUI.InitializeInventoryUI(inventorySize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.gameObject.GetComponent<Item>();
        if (item!=null)
        {
            // 가방이 꽉 찼다면 false로 아이템을 수집하지 않음
            if(Items.Count < inventorySize)
            {
                item.CollectItem(transform);
                //inventoryUI.RefreshUI();
            }
        }
    }

    public void OnInventoryToggle()
    {
        if (inventoryUI.gameObject.activeSelf)
            inventoryUI.Hide();
        else
            inventoryUI.Show();
    }
}