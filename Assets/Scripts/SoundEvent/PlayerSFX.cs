using UnityEngine;

public class PlayerWalkSound : MonoBehaviour
{
    [SerializeField, Range(0, 1f)] float volume;

    public void PlayWalkSound()
    {
        SoundManager.PlayRandomSound(SoundType.Footstep_Walk, volume);
    }
    public void PlayRunSound()
    {
        SoundManager.PlayRandomSound(SoundType.Footstep_Run);
    }
}
