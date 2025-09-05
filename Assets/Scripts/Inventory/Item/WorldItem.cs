using System.Collections;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("ItemValues")]
    public ItemData itemData;
    public int amount = 1;

    Rigidbody2D rb;
    SpriteRenderer sprite;

    [SerializeField] private float suckSpeed = 12f;     // 플레이어로 끌려가는 속도
    [SerializeField] private float arriveDistance = 0.25f; // 이 거리 이하로 오면 수집 완료
    [SerializeField] private float fadeTime = 0.15f;    // 수집 순간 페이드아웃 시간
    [SerializeField] private float absortableTime = 0.5f;

    public WorldItem(ItemData data) => itemData = data;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void SetItemData(ItemData data)
    {
        if (data == null) return;

        itemData = data;
        gameObject.name = itemData.name;
        sprite.sprite = itemData.Icon;

        amount = 1;
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
