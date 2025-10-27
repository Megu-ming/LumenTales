using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] AllItemData resolver;
    [SerializeField] SpawnData spawnData;
    PlayerSpawnPoint currentSpawnPoint = PlayerSpawnPoint.Default;

    [SerializeField] UIManager uiManagerPrefab;
    [SerializeField] DataManager dataManagerPrefab;

    UIManager ui;
    public UIManager GetUIManager() => ui;
    DataManager data;
    public DataManager GetDataManager() => data;

    SceneBase currentScene;
    Player player;

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

    private void Start()
    {
        InitSingleton();

        Play();
    }

    void Play()
    {
        if (ui is null && data is null)
        {
            ui = Instantiate(uiManagerPrefab);
            data = Instantiate(dataManagerPrefab);
        }

        ui.Init(player);
        data.Init(player);

        CurrentScene.Init();
    }

    public void SceneStart()
    {
        Play();
    }

    public void SetSpawnPoint(PlayerSpawnPoint point)
    {
        currentSpawnPoint = point;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnData.spawnPoint[(int)currentSpawnPoint];
    }

    // ---------- Scene ¿Ãµø ----------
    public void LoadScene(SceneType type)
    {
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
        data.InjectIntoCurrentPlayer(resolver);
    }

    public void SaveData()
    {
        data.BackupCurrentSlot();
        data.SaveAll();
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
