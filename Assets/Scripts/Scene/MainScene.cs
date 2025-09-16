using Unity.Cinemachine;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public static MainScene I {  get; private set; }

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
        Player.transform.position = spawnPos.position;
    }
}
