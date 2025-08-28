using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Item item;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        item = collision.gameObject.GetComponent<Item>();
        if (item!=null)
        {
            // ������ ȹ�� ���� ����
            Debug.Log("Item Collected: " + item.name);
            Destroy(item.gameObject, 1f);
        }
    }
}
