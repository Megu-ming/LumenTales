using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 2f;
    [SerializeField] float damage = 15f;
    [SerializeField] LayerMask targetMask;

    Vector2 dir;
    float life;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.SetParent(null, false);
        transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        life = lifeTime;
        if(rb) rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 from, Vector2 to)
    {
        transform.position = from;
        
        dir = (to - from).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (rb) rb.MoveRotation(angle);
        Debug.DrawLine(from, to, Color.red, 1f);
        if (rb) rb.linearVelocity = dir * speed;

        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //if (rb) rb.MoveRotation(angle);
    }

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f)
        { Destroy(gameObject); return; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            var pc = Player.instance.Status;
            pc.Hit(damage, pc.knockBack);
        }
    }
}
