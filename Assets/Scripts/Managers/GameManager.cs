using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] AllItemData resolver;

    SceneBase currentScene;

    [Header("Quest Manager")]
    public QuestManager questManager = new QuestManager();

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
        DataManager.instance.InjectIntoCurrentPlayer(resolver);
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
