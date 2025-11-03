using Unity.VisualScripting;
using UnityEngine;

public class PlaySoundBehaviour : StateMachineBehaviour
{
    public SfxType sfxType;
    public bool updateOnState;
    public bool updateOnStateMachine;
    public bool valueOnEnter, valueOnExit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(updateOnState && valueOnEnter)
        {
            SoundManager.PlayRandomSFX(sfxType);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState && valueOnExit)
        {
            SoundManager.PlayRandomSFX(sfxType);
        }
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && valueOnEnter)
        {
            SoundManager.PlayRandomSFX(sfxType);
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && valueOnExit)
        {
            SoundManager.PlayRandomSFX(sfxType);
        }
    }
}
