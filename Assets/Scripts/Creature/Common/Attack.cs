using UnityEngine;

public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Status otherStatus = collision.GetComponentInParent<Status>();
        Status myStatus = GetComponentInParent<Status>();
        int damage = myStatus.Damage;
        Vector2 knockBack = myStatus.knockBack;

        if (otherStatus != null)
        {
            bool gotHit = otherStatus.Hit(damage, knockBack);

            if (gotHit)
                Debug.Log("Hit " + collision.name + " for " + damage);
        }
    }
}
