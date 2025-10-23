using System.Collections;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    BossController boss;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.transform.root.GetComponent<BossController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 바라보고
        boss.LookAtPlayer();

		// 잠깐 대기
		float dist = boss.WaitForCalculateDistance();
		// 플레이어와 거리 판단 -> false면 이동
		if (boss.TryTriggerPattern(dist) is false)
        { 
            // 플레이어쪽으로 이동
            boss.MoveToPlayer(); 
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
