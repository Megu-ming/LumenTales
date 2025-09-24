// Player.cs
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Player : MonoBehaviour
{
    // ─── Singleton ──────────────────────────────────────────────────────────────
    public static Player instance { get; private set; }

    // ─── Cached Components (필요한 것만 캐싱) ───────────────────────────────────
    Rigidbody2D rb;
    Animator animator;
    PlayerStatus status;

#if ENABLE_INPUT_SYSTEM
    [Header("Optional")]
    [SerializeField] PlayerInput playerInput;
#endif

    [Header("Optional")]
    [Tooltip("로딩/페이드 동안 비활성화할 컨트롤 스크립트들(PlayerController 등)")]
    [SerializeField] Behaviour[] controlScriptsToToggle;

    // ─── Lifecycle ──────────────────────────────────────────────────────────────
    void Awake()
    {
        InitSingleton();

        // GameManager가 GameObject 레퍼런스로 접근하는 경우를 대비
        if (GameManager.instance) GameManager.instance.Player = gameObject;

        // 태그 보정(트리거/포털 인식용)
        if (CompareTag("Untagged")) gameObject.tag = "Player";
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
