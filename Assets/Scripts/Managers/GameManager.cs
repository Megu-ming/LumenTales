using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameObject container;

    static GameManager instance;
    public static GameManager Instance 
    { 
        get
        {
            if(!instance)
            {
                container = new GameObject();
                container.name = "GameManager";
                container.AddComponent<GameManager>();

                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

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
    }
    [SerializeField] GameObject UIRootPrefab;

    public PlayerStatus GetStatus() => Player.GetComponent<PlayerStatus>();

    public void Init()
    {
        playerInstance = Instantiate(playerPrefab);
    }

    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Prefab/Player/Player.prefab");
        UIRootPrefab = Resources.Load<GameObject>("Prefab/UI/UIRoot.prefab");
    }

    private void OnApplicationQuit()
    {
        // 데이터 저장
    }
}
