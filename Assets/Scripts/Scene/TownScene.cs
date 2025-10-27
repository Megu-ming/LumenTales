using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownScene : SceneBase
{
    [SerializeField] UIRoot uiRoot;
    [SerializeField] Store storeNPC;

    protected void Start()
    {
        GameManager.instance.SceneStart();
        uiRoot = GameManager.instance.GetUIManager().uiRoot;
    }

    public override void Init()
    {
        base.Init();
        sceneType = SceneType.Town;

        player.Init();
        uiRoot.Init(GameManager.instance.GetUIManager(), player);
        storeNPC.Init(GameManager.instance.GetUIManager());

        SpawnAndTrackingPlayer();

        GameManager.instance?.LoadData();
    }
}
