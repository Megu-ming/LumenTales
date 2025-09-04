using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("ItemValues")]
    public ItemData itemData;
    private ItemType itemType;

    public string ItemName { get { return gameObject.name; } set { gameObject.name = value; } }
    [SerializeField] private int goldAmount;
    [SerializeField] private string description;

    Rigidbody2D rb;
    SpriteRenderer sprite;
    public Sprite Sprite { get { return sprite.sprite; } set { sprite.sprite = value; } }

    [SerializeField] private float suckSpeed = 12f;     // 플레이어로 끌려가는 속도
    [SerializeField] private float arriveDistance = 0.25f; // 이 거리 이하로 오면 수집 완료
    [SerializeField] private float fadeTime = 0.15f;    // 수집 순간 페이드아웃 시간
    [SerializeField] private float absortableTime = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
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
                    ItemName = itemData.itemName;
                    Sprite = itemData.icon;
                    goldAmount = Random.Range(itemData.minGoldPrice, itemData.maxGoldPrice + 1);
                    description = itemData.description;
                    break;
                default:
                    ItemName = itemData.itemName;
                    Sprite  = itemData.icon;
                    goldAmount = itemData.goldAmount;
                    description = itemData.description;
                    break;
            }       
        }
    }

    public void CollectItem(Transform player)
    {
        StartCoroutine(CollectDelayRoutine(player, absortableTime));
    }
    private IEnumerator CollectDelayRoutine(Transform player, float delay)
    {
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(SuckToPlayer(player));
    }

    private IEnumerator SuckToPlayer(Transform player)
    {
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }

        while (player != null)
        {
            // 플레이어 방향으로 이동
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * suckSpeed * Time.deltaTime;

            // 가까워지면 수집 처리
            if (Vector3.Distance(transform.position, player.position) <= arriveDistance)
                break;

            yield return null;
        }

        // TODO : Add to Inventory
        InventoryController ic = player.GetComponent<InventoryController>();
        if (ic != null) 
        {
            switch (itemType)
            {
                case ItemType.Gold:
                    ic.GoldAmount += goldAmount;
                    break;
                case ItemType.Other:
                    if (ic.Items.TryGetValue(ItemName, out int current))
                    {
                        ic.Items[ItemName] = current + 1;
                        Debug.Log($"{ItemName}을 {current + 1}개 가지고있습니다");
                    }
                    else
                    {
                        ic.Items[ItemName] = 1;
                        Debug.Log($"{ItemName}을 처음 획득했습니다");
                    }
                    break;
                default:
                    break;
            }
        }

        // fadeout
        if (sprite != null)
        {
            Color c = sprite.color;
            float t = 0;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                float k = 1f - (t / fadeTime);
                sprite.color = new Color(c.r, c.g, c.b, k);
                yield return null;
            }
        }

        Destroy(gameObject);
    }
}
