using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public void PlayWalkSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Footstep_Walk);
    }
    public void PlayRunSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Footstep_Run);
    }
    public void PlayJumpSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Jump);
    }
    public void PlayLandSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Land);
    }
    public void PlayAttackSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Attack);
    }
    public void PlayHurtSound()
    {
        SoundManager.PlayRandomSFX(SfxType.Hurt);
    }
}
