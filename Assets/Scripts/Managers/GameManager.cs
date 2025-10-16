using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] AllItemData resolver;
    [SerializeField] SpawnData spawnData;
    PlayerSpawnPoint currentSpawnPoint = PlayerSpawnPoint.Default;

    SceneBase currentScene;

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

    public void SetSpawnPoint(PlayerSpawnPoint point)
    {
        currentSpawnPoint = point;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnData.spawnPoint[(int)currentSpawnPoint];
    }

    public void LoadScene(SceneType type)
    {
        if(type == SceneType.Menu)
        {
            Destroy(Player.instance?.gameObject);
        }
        SceneManager.LoadScene((int)type);
    }

    public void InjectData()
    {
        if(DataManager.instance)
            DataManager.instance.InjectIntoCurrentPlayer(resolver);
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
