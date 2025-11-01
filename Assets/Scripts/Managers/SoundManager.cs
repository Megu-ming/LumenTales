using System;
using UnityEngine;

public enum SoundType
{
    ButtonClick,
    PlayerAttack,
    PlayerHurt,
    EnemyHurt,
    EnemyDie,
    ItemPickup,
    LevelUp,
    MenuBGM,
    TownBGM,
    BattleBGM,
    BossBGM,
    Footstep_Walk,
    Footstep_Run,
}

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    [SerializeField] AudioSource BGMAudioSource;
    [SerializeField] AudioSource SFXAudioSource;

    private void Awake()
    {
        instance = this;
    }

    public static void PlayRandomSound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.SFXAudioSource.PlayOneShot(randomClip, volume);
    }
    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.BGMAudioSource.Play();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for(int i=0;i<names.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] AudioClip[] sounds;
}