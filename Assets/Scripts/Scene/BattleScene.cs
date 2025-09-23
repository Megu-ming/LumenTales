using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    [SerializeField] CinemachineCamera cam;
    [SerializeField] Transform spawnPos;
    [SerializeField] Texture2D cursorTexture;

    public GameObject Player;

    private void Awake()
    {
        sceneType = SceneType.Battle;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(cursorTexture, new Vector2(), CursorMode.Auto);
        cam.Target.TrackingTarget = Player.transform;
    }

    private void Start()
    {
        DataManager.Instance.LoadGameData();

        Player.transform.position = spawnPos.position;
    }

    private void OnDisable()
    {
        DataManager.Instance.SaveGameData();
        Debug.Log("Save Complete");
    }
}
