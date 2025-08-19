using UnityEngine;

public class Attack : MonoBehaviour
{
    private int attackDamage = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Status status = collision.GetComponentInParent<Status>();

        if(status != null)
        {
            bool gotHit = status.Hit(attackDamage);

            if (gotHit)
                Debug.Log("Hit " + collision.name + " for " + attackDamage);
        }
    }
}
