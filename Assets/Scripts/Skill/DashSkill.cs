using System.Collections;
using UnityEngine;

public class DashSkill : SkillBase
{
    [SerializeField] float dashSpeed = 15f;
    private float dashDuration = 0.1f;
    private float defaultSpeed;

    protected override void UseSkill()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        var pc = Player.instance.PlayerController;
        pc.IsDash = true;
        pc.IsSprint = false;
        defaultSpeed = pc.moveSpeed;
        pc.moveSpeed = dashSpeed;
        pc.SetGravity(0);
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(dashDuration);
        pc.IsDash = false;
        pc.moveSpeed = defaultSpeed;
        pc.SetGravity(3);
        Time.timeScale = 1f;
        yield break;
    }
}
