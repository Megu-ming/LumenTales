using UnityEngine;

/// <summary>
/// 보스 기본 공격 클래스
/// </summary>
public class MeleeAttack : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] float meleeDamage;

    public void Init(Player player)
    {
        this.player = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var status = player.Status;

            float randDamage = Random.Range(meleeDamage - (meleeDamage * 0.1f), meleeDamage + (meleeDamage * 0.1f));
            status.Hit(randDamage, new Vector2(2, 0));
        }
    }
}
