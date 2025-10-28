using UnityEngine;

public class BossAnimTriggers : MonoBehaviour
{
    [SerializeField] Transform bossTransform;
    Animator animator;
    Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(Player player)
    {
        this.player = player;
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
    [SerializeField] MissileProjectile missile;
    [SerializeField] Transform missileParent;

    public void LaunchMissile()
    {
        missile.Init(player, missileParent);
        missile.Launch(missileParent.position, player.transform.position);
    }
    // -----------------------------------------
    // ------------ 레이저 특수패턴 ------------
    [Header("레이저 생성용")]
    [SerializeField] LaserProjectile laser;
    [SerializeField] Transform laserParent;

    public void LaunchLaser()
    {
        laser.Init(player);
        if (bossTransform.localScale.x == -1)
            laser.transform.localScale = new Vector2(-1, 1);
        laser.Launch(laserParent.position, player.transform.position);
        animator.SetBool("LaserEnd", false);
    }

    public void EndLaser()
    {
        animator.SetBool("LaserEnd", true);
        laser.gameObject.SetActive(false);
    }
    // -----------------------------------------
}
