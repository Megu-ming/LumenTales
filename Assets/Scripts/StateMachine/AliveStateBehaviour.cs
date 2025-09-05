using System;
using UnityEngine;

public class AliveStateBehaviour : StateMachineBehaviour
{
    public float fadeinDuration = 0.5f;
    private float elapsedTime = 0f;
    Color startColor;
    SpriteRenderer sprite;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (animator.GetBool(AnimationStrings.isAlive))
        {
            sprite = animator.GetComponent<SpriteRenderer>();
            startColor = sprite.color;
            elapsedTime = 0f;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;

        float a = Mathf.Clamp01(elapsedTime / fadeinDuration);
        sprite.color = sprite ? new Color(startColor.r, startColor.g, startColor.b, a) : startColor;
    }
}
