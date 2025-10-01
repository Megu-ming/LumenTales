using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Battle;

        if (GameObject.Find("CMCam").TryGetComponent<CinemachineCamera>(out cam) && Player.instance is not null)
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
        UIManager.instance.InitUI();

        GameManager.instance.InjectData();
    }
}
