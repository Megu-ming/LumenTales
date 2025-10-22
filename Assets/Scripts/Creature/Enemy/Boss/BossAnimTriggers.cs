using UnityEngine;

public class BossAnimTriggers : MonoBehaviour
{
    [SerializeField] Transform bossTransform;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ------------ 기본 공격 ------------
    [SerializeField] Collider2D meleeCollider;

    public void ActiveMelee()
    {
        meleeCollider.enabled = true;
    }

    public void DeactiveMelee()
    {
        meleeCollider.enabled = false;
    }
    // -----------------------------------
    // ------------ 미사일 특수패턴 ------------
    [Header("미사일 생성용")]
    [SerializeField] MissileProjectile missilePrefab;
    [SerializeField] Transform missileParent;

    public void LaunchMissile()
    {
        var missile = Instantiate(missilePrefab, missileParent);
        missile.Launch(missileParent.position, Player.instance.transform.position);
    }
    // -----------------------------------------
    // ------------ 레이저 특수패턴 ------------
    [Header("레이저 생성용")]
    [SerializeField] LaserProjectile laserPrefab;
    [SerializeField] Transform laserParent;
    LaserProjectile laser;

    public void LaunchLaser()
    {
        laser = Instantiate(laserPrefab, laserParent);
        if (bossTransform.localScale.x == -1)
            laser.transform.localScale = new Vector2(-1, 1);
        laser.Launch(laserParent.position, Player.instance.transform.position);
        animator.SetBool("LaserEnd", false);
    }

    public void EndLaser()
    {
        animator.SetBool("LaserEnd", true);
        Destroy(laser.gameObject);
        laser = null;
    }
    // -----------------------------------------
}
