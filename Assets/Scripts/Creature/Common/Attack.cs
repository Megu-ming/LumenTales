using UnityEngine;

/// <summary>
/// 모든 기본 공격 클래스
/// </summary>
public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Status otherStatus = collision.GetComponentInParent<Status>();
        Status myStatus = GetComponentInParent<Status>();
        float damage = myStatus.BaseAtkDamage;
        if(myStatus is PlayerStatus playerStatus)
        {
            damage = playerStatus.FinalRandomDamage;
        }
        Vector2 knockBack = myStatus.knockBack;

        if (otherStatus != null)
        {
            float randDamage = Random.Range(damage - 5f, damage + 5f);
            bool gotHit = otherStatus.Hit(randDamage, knockBack);

            if (gotHit)
                Debug.Log("Hit " + collision.name + " for " + damage);
        }
    }
}