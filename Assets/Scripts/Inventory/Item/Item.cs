using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("ItemValues")]
    public ItemData itemData;
    private ItemType itemType;

    [SerializeField] private int price;
    [SerializeField] private string description;

    [SerializeField] float popForce = 2f;
    Rigidbody2D rb;
    SpriteRenderer sprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, popForce);
    }

    public void Init(ItemData inData)
    {
        itemData = inData;

        if (itemData == null)
            Debug.LogError($"ItemData is not assigned in {gameObject.name}");
        else
        {
            itemType = itemData.itemType;
            switch(itemType)
            {
                case ItemType.Gold:
                    gameObject.name = itemData.itemName;
                    sprite.sprite = itemData.icon;
                    price = Random.Range(itemData.minGoldPrice, itemData.maxGoldPrice + 1);
                    description = itemData.description;
                    break;
                default:
                    gameObject.name = itemData.itemName;
                    sprite.sprite = itemData.icon;
                    price = itemData.price;
                    description = itemData.description;
                    break;
            }       
        }
    }
}
