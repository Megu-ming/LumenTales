using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    [SerializeField] UIRoot uiRoot;

    protected void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        sceneType = SceneType.Battle;

        player.Init();
        uiRoot.Init(GameManager.instance.GetUIManager(), player);

        SpawnAndTrackingPlayer();

        GameManager.instance.LoadData();

        //if (Player.instance is not null)
        //    Player.instance.spotLight.gameObject.SetActive(true);
    }
}
