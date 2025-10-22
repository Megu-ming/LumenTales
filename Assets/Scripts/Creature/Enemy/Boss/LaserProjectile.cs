using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] Vector2 knockback;

    float attackTick = 0.25f;
    float tickTimer = 0f;
    Vector2 dir;

    public void Launch(Vector2 from, Vector2 to)
    {
        dir = (to - from).normalized;

        transform.right = dir;

        Debug.DrawLine(from, to, Color.red, 2f);
    }

    private void OnDestroy()
    {
        Debug.Log("LaserDestroy");
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
            Player.instance.Status.Hit(damage, knockback);
            tickTimer = 0f;
        }
        tickTimer += Time.deltaTime;
    }
}
