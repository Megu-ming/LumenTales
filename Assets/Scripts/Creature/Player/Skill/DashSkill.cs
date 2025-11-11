using System.Collections;
using UnityEngine;

public class DashSkill : SkillBase
{
    [SerializeField] float dashSpeed = 15f;
    private float dashDuration = 0.1f;
    private float defaultSpeed;

    public override void Init(Player player)
    {
        base.Init(player);
    }

    public override void UseSkill()
    {
        view.SetCooldownOn(true);
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        var pc = player.PlayerController;
        pc.IsDash = true;
        pc.IsSprint = false;
        var ps = player.Status;
        ps.SetInvincible(true);
        defaultSpeed = pc.moveSpeed;
        pc.moveSpeed = dashSpeed;
        pc.SetGravity(0);
        yield return new WaitForSeconds(dashDuration);
        pc.IsDash = false;
        ps.SetInvincible(false);
        pc.moveSpeed = defaultSpeed;
        pc.SetGravity(3);
        yield break;
    }
}
