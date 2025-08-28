using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public int damage;
    public float cooldown;
    public int maxHp;

    public Color spriteColor;
}
