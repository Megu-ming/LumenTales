using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameObject container;

    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "GameManager";
                instance = container.AddComponent<GameManager>();

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
        set => playerInstance = value;
    }
    [SerializeField] GameObject UIRootPrefab;

    public PlayerStatus GetStatus() => Player.GetComponent<PlayerStatus>();

    public void Init()
    {
        if(!Player) playerInstance = Instantiate(playerPrefab);
    }

    private void Awake()
    {
        
    }

    private void OnApplicationQuit()
    {
        // 데이터 저장
    }
}
