using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    [SerializeField] GameObject playerPrefab;
    GameObject playerInstance;
    public GameObject Player
    {
        get => playerInstance;
        set => playerInstance = value;
    }
    [SerializeField] GameObject UIRootPrefab;

    public PlayerStatus GetStatus() => Player.GetComponent<PlayerStatus>();


    private void Awake()
    {
        InitSingleton();
    }

    private void InitSingleton()
    {
        if (!instance) instance = this;
        else if (instance != this) Destroy(instance.gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
