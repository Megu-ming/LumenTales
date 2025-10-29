using Unity.Cinemachine;
using UnityEngine;

public class SceneBase : MonoBehaviour
{
    [SerializeField] protected Player player;

    protected SceneType sceneType;

    int statusPoint;
    public int StatusPoint
    {
        get => statusPoint;
        set => statusPoint = value;
    }

    [Header("SceneBase Option")]
    [SerializeField] protected CinemachineCamera cam;   
    [SerializeField] Texture2D cursorTexture;

    public virtual void Init()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);
    }

    public void SpawnAndTrackingPlayer()
    {
        if (player != null)
        {
            if (cam != null)
            {
                cam.Target.TrackingTarget = player.transform;
                if (GameManager.Instance != null)
                {
                    player.transform.position = GameManager.Instance.GetSpawnPosition();
                }

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
        var status = player.Status;
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
