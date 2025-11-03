using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum BgmType
{
    Menu,
    Town,
    Battle,
    Boss,
}

public enum SfxType
{
    Footstep_Walk,  // 걷기 발자국
    Footstep_Run,   // 뛰기 발자국
    Attack,         // 공격
    Hurt,           // 피격
    Jump,           // 점프
    Land,           // 착지
    // 필요하면 계속 추가
    UI_Hover,      // UI 호버
    UI_Click,      // UI 클릭
}

public enum EAudioMixerType { Master, BGM, SFX }

[DisallowMultipleComponent]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources (자식 오브젝트에 부착)")]
    [SerializeField] private AudioSource bgmSource; // loop 전용, clip+Play 사용
    [SerializeField] private AudioSource sfxSource; // 원샷 전용, PlayOneShot 사용

    [SerializeField] private AudioMixer audioMixer;

    [Header("BGM 테이블 (타입당 1개 클립)")]
    [SerializeField] private BgmEntry[] bgmTable;

    [Header("SFX 테이블 (타입당 여러 개 클립)")]
    [SerializeField] private SfxEntry[] sfxTable;

    // 내부 상태
    private Coroutine bgmFadeCo;

    #region Singleton & Lifetime
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        // 씬이 바뀌면 BGM 자동 정지(새 씬에서 PlayBGM을 부르면 자연스럽게 교체됨)
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        StopBGM(0.1f); // 짧게 페이드아웃하며 정지
    }
    #endregion

    #region Public API (Settings 연동)
    // 설정 메뉴에서 호출해줄 볼륨 세터
    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }
    #endregion

    #region BGM (단일 클립, loop, Play 사용)
    /// <summary>
    /// BGM 타입으로 교체 재생 (기본: 페이드 0.25초)
    /// </summary>
    public static void PlayBGM(BgmType type, float fadeSeconds = 0.25f)
    {
        if (instance == null || instance.bgmSource == null) return;

        var clip = instance.GetBgmClip(type);
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] BGM 클립이 비어있습니다: {type}");
            return;
        }

        // 같은 트랙이면 스킵
        if (instance.bgmSource.isPlaying && instance.bgmSource.clip == clip) return;

        if (instance.bgmFadeCo != null) instance.StopCoroutine(instance.bgmFadeCo);
        instance.bgmFadeCo = instance.StartCoroutine(instance.Co_SwapBgm(clip, fadeSeconds));
    }

    /// <summary>
    /// 현재 BGM 정지(옵션: 페이드)
    /// </summary>
    public static void StopBGM(float fadeSeconds = 0.15f)
    {
        if (instance == null || instance.bgmSource == null) return;
        if (!instance.bgmSource.isPlaying) return;

        if (instance.bgmFadeCo != null) instance.StopCoroutine(instance.bgmFadeCo);
        instance.bgmFadeCo = instance.StartCoroutine(instance.Co_StopBgm(fadeSeconds));
    }

    private IEnumerator Co_SwapBgm(AudioClip next, float fade)
    {
        // 페이드아웃
        if (bgmSource.isPlaying && fade > 0f)
        {
            float start = bgmSource.volume;
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(start, 0f, t / fade);
                yield return null;
            }
        }

        // 교체 & 재생
        bgmSource.Stop();
        bgmSource.clip = next;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // 페이드인
        //float target = bgmBaseVolume;
        float target = 1f;
        if (fade > 0f)
        {
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0f, target, t / fade);
                yield return null;
            }
        }
        bgmSource.volume = target;
        bgmFadeCo = null;
    }

    private IEnumerator Co_StopBgm(float fade)
    {
        float start = bgmSource.volume;
        if (fade > 0f)
        {
            for (float t = 0; t < fade; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(start, 0f, t / fade);
                yield return null;
            }
        }
        bgmSource.Stop();
        //bgmSource.volume = bgmBaseVolume;
        bgmFadeCo = null;
    }

    private AudioClip GetBgmClip(BgmType type)
    {
        int idx = (int)type;
        if (bgmTable == null || idx < 0 || idx >= bgmTable.Length) return null;
        return bgmTable[idx].clip;
    }
    #endregion

    #region SFX (여러 클립 중 랜덤, PlayOneShot 사용)
    /// <summary>
    /// 타입에 등록된 여러 클립 중 하나를 랜덤 재생
    /// </summary>
    public static void PlayRandomSFX(SfxType type, float volumeScale = 1f, bool randomizePitch = false, float pitchMin = 0.96f, float pitchMax = 1.04f)
    {
        if (instance == null || instance.sfxSource == null) return;

        var clip = instance.GetRandomSfxClip(type);
        if (clip == null) return;

        // 원래 피치 복구 후, 옵션에 따라 랜덤 피치
        float oldPitch = instance.sfxSource.pitch;
        if (randomizePitch) instance.sfxSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);

        instance.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));

        instance.sfxSource.pitch = oldPitch;
    }

    private AudioClip GetRandomSfxClip(SfxType type)
    {
        int idx = (int)type;
        if (sfxTable == null || idx < 0 || idx >= sfxTable.Length) return null;
        var list = sfxTable[idx].clips;
        if (list == null || list.Length == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Length)];
    }
    #endregion

#if UNITY_EDITOR
    // 인스펙터에서 enum 개수 변화에 따라 배열 길이와 이름 라벨 자동 동기화
    private void OnValidate()
    {
        // BGM
        var bgmNames = Enum.GetNames(typeof(BgmType));
        if (bgmTable == null || bgmTable.Length != bgmNames.Length)
            Array.Resize(ref bgmTable, bgmNames.Length);
        for (int i = 0; i < bgmNames.Length; i++)
        {
            if (bgmTable[i].label != bgmNames[i])
                bgmTable[i].label = bgmNames[i];
        }

        // SFX
        var sfxNames = Enum.GetNames(typeof(SfxType));
        if (sfxTable == null || sfxTable.Length != sfxNames.Length)
            Array.Resize(ref sfxTable, sfxNames.Length);
        for (int i = 0; i < sfxNames.Length; i++)
        {
            if (sfxTable[i].label != sfxNames[i])
                sfxTable[i].label = sfxNames[i];
        }
    }
#endif

    private void ChangeVolume(float volume)
    {
        instance.SetAudioVolume(EAudioMixerType.BGM, volume);
    }
}

[Serializable]
public struct BgmEntry
{
    [HideInInspector] public string label;
    public AudioClip clip;  // 타입당 1개
}

[Serializable]
public struct SfxEntry
{
    [HideInInspector] public string label;
    public AudioClip[] clips; // 타입당 여러 개
}
