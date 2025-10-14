using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameData
{
    // 플레이어 정보들
    public int currentSlot = -1;
    public SlotData[] saveSlots = new SlotData[5];
}

[Serializable]
public class SlotData
{
    public bool exists;

    public long createdAtUnix;
    public long updatedAtUnix;

    public PlayerSummary player;        // 스테이터스 요약
    public InventorySnapshot inventory; // 인벤/장비 요약
    public QuestSnapshot questSnapshot; // 퀘스트 요약
}

[Serializable]
public class SlotMeta
{
    public int slotIndexBased;  // 1~5
    public bool exists;         // 저장 존재 유무
    public long updateAtUnix;   // 마지막 저장 시각
    public int level;           // 표시용 레벨

    public string DisplayTime
    {
        get
        {
            if (updateAtUnix <= 0) return "";
            var dt = DateTimeOffset.FromUnixTimeSeconds(updateAtUnix).ToLocalTime().DateTime;
            return dt.ToString("yyyy-MM-dd HH:mm");
        }
    }

    public string TitleLine => exists ? $"Slot {slotIndexBased}  LV.{level}\n {DisplayTime}" : "(Empty Slot)";

    public static SlotMeta From(SlotData sd, int indexBased)
    {
        bool has = sd != null && sd.exists;
        return new SlotMeta
        {
            slotIndexBased = indexBased,
            exists = has,
            updateAtUnix = has ? sd.updatedAtUnix : 0,
            level = has && sd.player != null ? sd.player.level : 1
        };
    }
}

[Serializable]
public class PlayerSummary
{
    public int level = 1;
    public int currentExp = 0;
    public int maxExp = 20;

    public float spAddedStr = 0;
    public float spAddedAgi = 0;
    public float spAddedLuk = 0;
    public int statusPoint = 0;
}

[Serializable]
public class  InventorySnapshot
{
    public int gold = 0;

    // 빈칸 저장x
    public List<InventoryItemEntry> itemEntry = new List<InventoryItemEntry>();
    public List<EquippedEntry> equippedEntry = new List<EquippedEntry>();
}

public class QuestSnapshot
{
    public List<Quest> inProgressQuests = new List<Quest>();
    public List<QuestData> completedQuests = new List<QuestData>();
}

[Serializable]
public class InventoryItemEntry
{
    public int slotIndex;
    public string itemName;
    public int amount = 1;
}

[Serializable]
public class EquippedEntry
{
    public EquipmentSlotType slotType;
    public string itemName;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    string gameDataFileName = "LumenTalesData.json";

    public GameData Current = new GameData();

    public Action<InventorySnapshot> ApplyInventorySnapshot;       // 로드시 인벤/장비 적용
    public Action<PlayerSummary> ApplyPlayerSummary;           // 로드시 스탯 적용

    public event Action OnSlotsChanged;

    string FilePath => Path.Combine(Application.persistentDataPath, gameDataFileName);

    private void Awake()
    {
        InitSingleton();
        LoadAll();     // 앱 시작 시 로드
    }

    public void LoadAll(int slotSize = 5)
    {
        if (File.Exists(FilePath))
        {
            string fromJsonData = File.ReadAllText(FilePath);
            Current = JsonUtility.FromJson<GameData>(fromJsonData);
            Debug.Log(gameDataFileName + $"불러오기 성공");
        }
        else Current = new GameData();

        if(Current.saveSlots == null || Current.saveSlots.Length!= slotSize)
        {
            var fixedSlots = new SlotData[slotSize];
            if(Current.saveSlots !=null)
            {
                for(int i=0;i<Mathf.Min(slotSize, Current.saveSlots.Length);i++)
                    fixedSlots[i]=Current.saveSlots[i];
            }
            Current.saveSlots = fixedSlots;
        }
    }

    public void SaveAll() 
    {
        string toJsonData = JsonUtility.ToJson(Current, true);
        File.WriteAllText(FilePath, toJsonData);
        OnSlotsChanged?.Invoke();
    }

    public SlotMeta[] GetSlotMetas()
    {
        var arr = new SlotMeta[Current.saveSlots.Length];
        for (int i = 0; i < arr.Length; i++)
            arr[i] = SlotMeta.From(Current.saveSlots[i], i + 1);
        return arr;
    }

    public void NewGameAtSlot(int slotIndex)
    {
        long now = DateTimeOffset.Now.ToUnixTimeSeconds();

        var slot = new SlotData
        {
            exists = true,
            createdAtUnix = now,
            updatedAtUnix = now,
            player = new PlayerSummary { level = 1, maxExp = 20 },
            inventory = new InventorySnapshot { gold = 0 },
        };

        Current.currentSlot = slotIndex;
        Current.saveSlots[slotIndex] = slot;
        SaveAll();
    }

    public bool ContinueAtSlot(int slotIndex)
    {
        var slot = Current.saveSlots[slotIndex];
        if(slot == null || !slot.exists) return false;

        Current.currentSlot = slotIndex;

        return true;
    }

    public void DeleteSlot(int slotIndex)
    {
        Current.saveSlots[slotIndex] = new SlotData();
        Current.currentSlot = -1;

        SaveAll();
    }

    public void InjectIntoCurrentPlayer(IItemResolver resolver)
    {
        if (Current.currentSlot < 0) return;
        var slot = Current.saveSlots[Current.currentSlot];
        if (slot == null || !slot.exists) return;
        if (Player.instance == null) return;
        if(GameManager.instance is null && GameManager.instance.questManager is null) return;

        Player.instance.ApplySummary(slot.player);
        Player.instance.InventoryController.LoadFromSnapshot(slot.inventory, resolver);
        GameManager.instance.questManager.ApplyQuest(slot.questSnapshot);
    }

    public void BackupCurrentSlot()
    {
        if (Current.currentSlot < 0) return;
        var slot = Current.saveSlots[Current.currentSlot];
        if (slot == null || !slot.exists) return;

        if(Player.instance is not null && GameManager.instance is not null)
        {
            slot.player = Player.instance.GetPlayerSummary();
            slot.inventory = Player.instance.GetInventorySnapshot();
            slot.questSnapshot = GameManager.instance.questManager.GetQuestSnapshot();
        }

        slot.updatedAtUnix = DateTimeOffset.Now.ToUnixTimeSeconds();

        SaveAll();
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}