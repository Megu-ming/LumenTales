using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockBack = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Status status = collision.GetComponentInParent<Status>();

        if(status != null)
        {
            bool gotHit = status.Hit(attackDamage, knockBack);

            if (gotHit)
                Debug.Log("Hit " + collision.name + " for " + attackDamage);
        }
    }
}
