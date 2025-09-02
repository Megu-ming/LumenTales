using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Item item;
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
}