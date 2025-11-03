using System;
using System.Collections;
using UnityEngine;
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

[DisallowMultipleComponent]
public class SoundManager : MonoBehaviour
{
    public static SoundManager I { get; private set; }

    [Header("Audio Sources (자식 오브젝트에 부착)")]
    [SerializeField] private AudioSource bgmSource; // loop 전용, clip+Play 사용
    [SerializeField] private AudioSource sfxSource; // 원샷 전용, PlayOneShot 사용

    [Header("BGM 테이블 (타입당 1개 클립)")]
    [SerializeField] private BgmEntry[] bgmTable;

    [Header("SFX 테이블 (타입당 여러 개 클립)")]
    [SerializeField] private SfxEntry[] sfxTable;

    // 내부 상태
    private Coroutine bgmFadeCo;
    private float bgmBaseVolume = 1f; // 페이드 후 원복용
    private float sfxBaseVolume = 1f;

    #region Singleton & Lifetime
    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        if (bgmSource != null) bgmBaseVolume = bgmSource.volume;
        if (sfxSource != null) sfxBaseVolume = sfxSource.volume;
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
    public static void SetBgmVolume(float v)
    {
        if (I?.bgmSource == null) return;
        I.bgmBaseVolume = Mathf.Clamp01(v);
        I.bgmSource.volume = I.bgmBaseVolume;
    }

    public static void SetSfxVolume(float v)
    {
        if (I?.sfxSource == null) return;
        I.sfxBaseVolume = Mathf.Clamp01(v);
        // PlayOneShot은 인스펙터/오디오소스의 volume을 기본 스케일로 씀
        I.sfxSource.volume = I.sfxBaseVolume;
    }

    public static float GetBgmVolume() => I?.bgmSource ? I.bgmSource.volume : 0f;
    public static float GetSfxVolume() => I?.sfxSource ? I.sfxSource.volume : 0f;
    #endregion

    #region BGM (단일 클립, loop, Play 사용)
    /// <summary>
    /// BGM 타입으로 교체 재생 (기본: 페이드 0.25초)
    /// </summary>
    public static void PlayBGM(BgmType type, float fadeSeconds = 0.25f)
    {
        if (I == null || I.bgmSource == null) return;

        var clip = I.GetBgmClip(type);
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] BGM 클립이 비어있습니다: {type}");
            return;
        }

        // 같은 트랙이면 스킵
        if (I.bgmSource.isPlaying && I.bgmSource.clip == clip) return;

        if (I.bgmFadeCo != null) I.StopCoroutine(I.bgmFadeCo);
        I.bgmFadeCo = I.StartCoroutine(I.Co_SwapBgm(clip, fadeSeconds));
    }

    /// <summary>
    /// 현재 BGM 정지(옵션: 페이드)
    /// </summary>
    public static void StopBGM(float fadeSeconds = 0.15f)
    {
        if (I == null || I.bgmSource == null) return;
        if (!I.bgmSource.isPlaying) return;

        if (I.bgmFadeCo != null) I.StopCoroutine(I.bgmFadeCo);
        I.bgmFadeCo = I.StartCoroutine(I.Co_StopBgm(fadeSeconds));
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
        float target = bgmBaseVolume;
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
        bgmSource.volume = bgmBaseVolume;
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
        if (I == null || I.sfxSource == null) return;

        var clip = I.GetRandomSfxClip(type);
        if (clip == null) return;

        // 원래 피치 복구 후, 옵션에 따라 랜덤 피치
        float oldPitch = I.sfxSource.pitch;
        if (randomizePitch) I.sfxSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);

        I.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));

        I.sfxSource.pitch = oldPitch;
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
