using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    Animator animator;
    Player player;
    GameObject owner;
    [SerializeField] float damage;
    [SerializeField] Vector2 knockback;

    float attackTick = 0.1f;
    float tickTimer = 0f;
    Vector2 dir;

    public void Init(Player player, GameObject owner)
    {
        this.player = player;
        this.owner = owner;
    }

	public void Launch(Vector2 from, Vector2 to)
    {
        gameObject.SetActive(true);
        dir = (to - from).normalized;

        // 레이저 다른곳으로 쏘는것 방지
        transform.localScale = owner.transform.localScale;

        transform.right = dir;

        Debug.DrawLine(from, to, Color.red, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            tickTimer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && tickTimer >= attackTick)
        {
            player.Status.Hit(damage, knockback);
            tickTimer = 0f;
        }
        tickTimer += Time.deltaTime;
    }

    public void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}
