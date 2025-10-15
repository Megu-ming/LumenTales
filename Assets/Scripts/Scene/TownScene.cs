using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Town;

        if(GameObject.Find("CMCam").TryGetComponent<CinemachineCamera>(out cam) && Player.instance is not null)
        {
            cam.Target.TrackingTarget = Player.instance.transform;
        }
    }

    protected override void Start()
    {
        base.Start();

        InitScene();
    }

    public void InitScene()
    {
        Player.instance?.InventoryController.Init();
        UIManager.instance?.InitUI();

        GameManager.instance?.InjectData();
    }
}
