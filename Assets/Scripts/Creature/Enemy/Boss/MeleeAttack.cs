using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] Player player;
    public float meleeDamage = 10f;

    public void Init(Player player)
    {
        this.player = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var status = player.Status;
            status.Hit(meleeDamage, new Vector2(2, 0));
        }
    }
}
