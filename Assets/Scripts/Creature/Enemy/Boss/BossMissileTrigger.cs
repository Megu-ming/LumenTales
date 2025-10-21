using UnityEngine;

public class BossMissileTrigger : MonoBehaviour
{
    [Header("ÇÁ¸®ÆÕ")]
    [SerializeField] MissileProjectile missilePrefab;
    [SerializeField] Transform missileParent;

    public void LaunchMissile()
    {
        var missile = Instantiate(missilePrefab, missileParent);
        missile.Launch(missileParent.position, Player.instance.transform.position);
    }
}
