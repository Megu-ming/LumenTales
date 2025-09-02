using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("InventoryUISetting")]
    [SerializeField] UIInventory inventoryUI;
    [SerializeField] int inventorySize = 10;

    private Dictionary<string, int> items = new Dictionary<string, int>();
    public Dictionary<string, int> Items { get { return items; } }


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
            if(inventoryUI.IntoInventory(item, gameObject)) 
            {
                
                item.CollectItem(transform);
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