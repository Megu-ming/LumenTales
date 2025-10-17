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

    private void FixedUpdate()
    {
        cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Max(-1.42f, cam.transform.position.y), cam.transform.position.z);
    }

    public void InitScene()
    {
        Player.instance?.InventoryController.Init();
        UIManager.instance.InitUI();

        GameManager.instance.InjectData();

        if (Player.instance is not null)
            Player.instance.spotLight.gameObject.SetActive(true);
    }
}
