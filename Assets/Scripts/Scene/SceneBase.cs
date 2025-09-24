using Unity.Cinemachine;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    protected SceneType sceneType;

    int statusPoint;
    public int StatusPoint
    {
        get => statusPoint;
    }

    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform spawnPos;
    [SerializeField] Texture2D cursorTexture;

    public GameObject Player;

    protected virtual void Awake()
    {
        sceneType = SceneType.None;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);

        if(Player != null && cam != null)
        {
            cam.Target.TrackingTarget = Player.transform;
            Player.transform.position = spawnPos.position;
        }
    }

    protected virtual void Start()
    {
        //DataManager.instance.LoadGameData();

        GameManager.instance.Player = Player;
    }

    protected virtual void OnApplicationQuit()
    {
        DataManager.instance?.SaveGameData();
        Debug.Log("Save Complete");
    }

    public bool UseStatusPoint()
    {
        if (statusPoint > 0) { --statusPoint; return true; }
        else return false;
    }

    public void AddExp(int exp)
    {
        GameManager.instance.Player.TryGetComponent<PlayerStatus>(out var status);
        if(status != null)
        {
            status.CurrentExp += exp;
            if(status.CurrentExp>=status.MaxExp)
            {
                status.Level++;
                Debug.Log($"Player Level Up! : {status.Level}");
                statusPoint += 5;
                status.CurrentExp -= status.MaxExp;
                status.MaxExp *= 2;
                CharacterEvents.infoUIRefresh?.Invoke();
            }
        }
    }
}
