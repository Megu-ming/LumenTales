using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    Player player;

    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 2f;
    [SerializeField] float damage = 15f;
    [SerializeField] LayerMask targetMask;

    Transform parentTransform;
    Vector2 dir;
    float life;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    public void Init(Player player, Transform parent)
    {
        this.player = player;
        parentTransform = parent;
    }

    private void OnEnable()
    {
        life = lifeTime;
        if(rb) rb.linearVelocity = Vector2.zero;
    }

    public void Launch(Vector2 from, Vector2 to)
    {
        gameObject.SetActive(true);
        transform.SetParent(null, false);
        transform.localScale = Vector3.one;
        transform.position = from;
        
        dir = (to - from).normalized;

        transform.right = -dir;

        Debug.DrawLine(from, to, Color.red, 1f);
        if (rb) rb.linearVelocity = dir * speed;
    }

    private void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f)
        { 
            gameObject.SetActive(false);
            ResetPosition();
            return; 
        }
    }

    private void ResetPosition()
    {
        transform.SetParent(parentTransform);
        transform.position = new Vector2(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            var pc = player.Status;
            pc.Hit(damage, pc.knockBack);
        }
    }
}
