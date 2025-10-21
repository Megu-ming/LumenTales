using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 1;
    public float attackRange = 1.5f;

    BossController boss;
    Transform player;
    Rigidbody2D rb;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = Player.instance.transform;
        boss = animator.transform.root.GetComponent<BossController>();
        rb = animator.transform.root.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();

        float distance = Vector2.Distance(player.position, rb.position);

        if(boss.TryTriggerNextPattern(distance))
            return;

        boss.MoveToPlayer();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
