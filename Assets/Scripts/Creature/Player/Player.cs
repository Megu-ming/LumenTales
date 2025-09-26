// Player.cs
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
#endif

public class Player : MonoBehaviour
{
    // ─── Singleton ──────────────────────────────────────────────────────────────
    public static Player instance;

    // ─── Cached Components (필요한 것만 캐싱) ───────────────────────────────────
    public PlayerStatus Status { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public InventoryController InventoryController { get; private set; }

    // ─── Lifecycle ──────────────────────────────────────────────────────────────
    void Awake()
    {
        InitSingleton();
        Status = GetComponent<PlayerStatus>();
        PlayerController = GetComponent<PlayerController>();
        InventoryController = GetComponentInChildren<InventoryController>();

        // 태그 보정(트리거/포털 인식용)
        if (CompareTag("Untagged")) gameObject.tag = "Player";

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        GameManager.instance.InjectData();
    }

    private void OnDestroy()
    {
        DataManager.instance.BackupCurrentSlot();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 불러온 데이터 주입
    public void ApplySummary(PlayerSummary ps)
    {
        if (ps == null) return;

        Status.Level = ps.level;
        Status.CurrentExp = ps.currentExp;
        Status.MaxExp = ps.maxExp;
        Status.SpAddedStr = ps.spAddedStr;
        Status.SpAddedAgi = ps.spAddedAgi;
        Status.SpAddedLuk = ps.spAddedLuk;
    }

    /// <summary>
    /// 플레이어의 현재 저장가능한 수치들을 가져온다.
    /// </summary>
    /// <returns></returns>
    public PlayerSummary GetPlayerSummary()
    {
        var summary = new PlayerSummary();

        summary.level = Status.Level;
        summary.currentExp = Status.CurrentExp;
        summary.maxExp = Status.MaxExp;
        summary.spAddedStr = Status.SpAddedStr;
        summary.spAddedAgi = Status.SpAddedAgi;
        summary.spAddedLuk = Status.SpAddedLuk;

        return summary;
    }

    public InventorySnapshot GetInventorySnapshot()
    {
        var snapshot = new InventorySnapshot();
        snapshot.gold = InventoryController.Gold;
        snapshot.itemEntry = InventoryController.GetInvenSnapshot();
        snapshot.equippedEntry = InventoryController.GetEquipSnapshot();

        return snapshot;
    }

    // Singleton
    private void InitSingleton()
    {
        if (!instance)
        { instance = this; DontDestroyOnLoad(gameObject); }
        else if (instance != this) Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
