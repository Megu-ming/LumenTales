using Unity.Cinemachine;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    protected SceneType sceneType;

    int statusPoint;
    public int StatusPoint
    {
        get => statusPoint;
        set => statusPoint = value;
    }

    [Header("SceneBase Option")]
    [SerializeField] protected CinemachineCamera cam;   
    [SerializeField] Transform spawnPos;
    [SerializeField] Texture2D cursorTexture;

    protected virtual void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);
    }

    protected virtual void Start()
    {
        if (Player.instance != null)
        {
            var player = Player.instance?.gameObject;
            if (player != null && cam != null)
            {
                cam.Target.TrackingTarget = player.transform;
                player.transform.position = spawnPos.position;
            }
        }
    }

    public bool UseStatusPoint()
    {
        if (StatusPoint > 0) { --StatusPoint; return true; }
        else return false;
    }

    public void AddExp(int exp)
    {
        var status = Player.instance.Status;
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
