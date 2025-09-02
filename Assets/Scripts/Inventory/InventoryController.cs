using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("InventorySetting")]
    [SerializeField] UIInventory inventoryUI;
    [SerializeField] int inventorySize = 10;

    [SerializeField] Item item;

    private void Start()
    {
        if (inventoryUI == null)
            Debug.LogError("Inventory UI is not assigned in InventoryController.");
        else
            inventoryUI.InitializeInventoryUI(inventorySize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        item = collision.gameObject.GetComponent<Item>();
        if (item!=null)
        {
            // 아이템 획득 로직 구현
            Debug.Log("Item Collected: " + item.name);
            item.CollectItem(transform);
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