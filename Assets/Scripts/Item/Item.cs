using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] float popForce = 2f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, popForce);
    }
}
