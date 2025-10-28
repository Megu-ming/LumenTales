using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownScene : SceneBase
{
    [SerializeField] UIRoot uiRoot;
    [SerializeField] Store storeNPC;

    protected override void Start()
    {
        base.Start();

        player.Init();
        uiRoot.Init(player);
        storeNPC.Init(uiRoot);

        GameManager.instance?.SceneStart();
    }

    public override void Init()
    {
        base.Init();
        sceneType = SceneType.Town;

        SpawnAndTrackingPlayer();

        GameManager.instance?.LoadData();
    }
}
