using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Header("Status")]
    public int damage;
    public Vector2 knockBack;
    public float cooldown;
    public int maxHp;
    public bool isInvincible;
    public float invincibilityTime;

    [Header("EnemyController")]
    public float moveSpeed;
    public float walkStopRate;

    [Header("Item Drop")]
    public ItemDT itemDT;
}
