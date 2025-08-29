using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float elapsedTime = 0f;
    SpriteRenderer sprite;
    GameObject objToRemove;
    Color startColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime = 0f;
        sprite = animator.GetComponent<SpriteRenderer>();
        startColor = sprite.color;
        objToRemove = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;

        float newAlpha = startColor.a * (1 - (elapsedTime / fadeTime));

        sprite.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        if (elapsedTime > fadeTime)
        {
            if (objToRemove != null)
            {
                // object Pooling으로 삭제 방지
                // Destroy(objToRemove);
                EnemyController ec = objToRemove.transform.root.GetComponent<EnemyController>();
                if (ec != null) { ec.OnDead(); }
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}
}
