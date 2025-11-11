using System.Collections;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    BossController boss;
    float dist;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.transform.root.GetComponent<BossController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dist = boss.CalculateDistance();
        // 플레이어와 거리 판단 -> false면 이동
        if (boss.TryTriggerPattern(dist) is false)
        { 
            // 플레이어쪽으로 이동
            boss.MoveToPlayer(); 
        }
    }
}
