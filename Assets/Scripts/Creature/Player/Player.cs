using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ─── Singleton ──────────────────────────────────────────────────────────────
    public static Player instance;

    // ─── Cached Components (필요한 것만 캐싱) ───────────────────────────────────
    public PlayerStatus Status { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public InventoryController InventoryController { get; private set; }

    // ─── Quest ────────────────────────────────────────────────────────────────
   
    

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

    private void OnDestroy()
    {
        if(DataManager.instance != null)
            DataManager.instance.BackupCurrentSlot();
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
        if(GameManager.instance.CurrentScene != null)
            GameManager.instance.CurrentScene.StatusPoint = ps.statusPoint;

        if(UIManager.instance!=null)
        {
            UIManager.instance.OnExpChanged(Status.CurrentExp, Status.MaxExp);
        }
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
        if (GameManager.instance.CurrentScene != null)
            summary.statusPoint = GameManager.instance.CurrentScene.StatusPoint;

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
}