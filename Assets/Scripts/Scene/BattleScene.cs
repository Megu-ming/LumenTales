using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    public static BattleScene I {  get; private set; }

    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform spawnPos;
    [SerializeField] Texture2D cursorTexture;

    public GameObject Player;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);
        cam.Target.TrackingTarget = Player.transform;
    }

    private void Start()
    {
        DataManager.Instance.LoadGameData();

        Player.TryGetComponent<PlayerStatus>(out PlayerStatus ps);
        GameManager.Instance.SetStatus(ps);
        Player.transform.position = spawnPos.position;
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveGameData();
    }
}
