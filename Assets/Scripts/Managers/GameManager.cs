using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] AllItemData resolver;
    [SerializeField] SpawnData spawnData;
    PlayerSpawnPoint currentSpawnPoint = PlayerSpawnPoint.Default;

    [Header("--------------읽기 전용--------------")]
    [SerializeField] UIRoot uiRoot;
    public UIRoot GetUIRoot() => uiRoot;

    [SerializeField] DataManager data;
    public DataManager GetDataManager() => data;
    int currentSlotIndex = 0;
    public void SetCurrentSlotIndex(int index) => currentSlotIndex = index;

    SceneBase currentScene;
    Player player;

    // 게임이 진행 중인지 확인하는 불 변수
    bool isIngame;
    public bool IsIngame
    {
        get { return isIngame; }
        set { isIngame = value; }
    }

    public SceneBase CurrentScene
    {
        get
        {
            if(!currentScene)
            {
                currentScene = FindAnyObjectByType<SceneBase>();
            }
            return currentScene;
        }
    }

    private void Awake()
    {
        InitSingleton();
    }

    public void SceneStart()
    {
        player = FindAnyObjectByType<Player>();

        data = FindAnyObjectByType<DataManager>();
        if (data is not null)
        {
            data.Init(player, currentSlotIndex);
        }

        if (CurrentScene != null)
            CurrentScene.Init();
    }

    public void SetSpawnPoint(PlayerSpawnPoint point)
    {
        currentSpawnPoint = point;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnData.spawnPoint[(int)currentSpawnPoint];
    }

    // ---------- Scene 이동 ----------
    public void LoadScene(SceneType type)
    {
        uiRoot = null;
        data = null;
        SceneManager.LoadScene((int)type);
    }

    public void LoadSceneWithSave(SceneType type)
    {
        data.BackupCurrentSlot();
        data.SaveAll();
        LoadScene(type);
    }

    public void LoadSceneFromNewGame(int slotIndex)
    {
        data.NewGameAtSlot(slotIndex);
        LoadScene(SceneType.Town);
    }

    public void LoadSceneFromContine(int slotIndex)
    {
        data.ContinueAtSlot(slotIndex);
        LoadScene(SceneType.Town);
    }

    public void LoadData()
    {
        if (!isIngame)
            data.InjectIntoCurrentPlayer(resolver);
        else
            data.InjectPlayerIngameData(resolver);
    }

    public void SaveData()
    {
        data.BackupCurrentSlot();
        data.SaveAll();
    }

    private void InitSingleton()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            Instance = this; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
