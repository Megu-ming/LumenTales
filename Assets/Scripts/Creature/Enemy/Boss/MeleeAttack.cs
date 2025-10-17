using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float meleeDamage = 10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var status = Player.instance.Status;
            status.Hit(meleeDamage, new Vector2(2, 0));
        }
    }
}
