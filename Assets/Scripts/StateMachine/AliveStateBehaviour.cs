using UnityEngine;

public class AliveStateBehaviour : StateMachineBehaviour
{
    public bool isStateEnter;
    public bool isStateMachineEnter;
    SpriteRenderer sprite;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(isStateEnter)
        {
            sprite = animator.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
    }
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (isStateMachineEnter)
        {
            sprite = animator.GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
        }
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}
