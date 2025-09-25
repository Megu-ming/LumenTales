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
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
