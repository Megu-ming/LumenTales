using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    [SerializeField] UIRoot uiRoot;

    protected void Start()
    {
        player.Init();
        uiRoot.Init(player);

        GameManager.Instance?.SceneStart();
    }

    public override void Init()
    {
        base.Init();
        sceneType = SceneType.Battle;

        SpawnAndTrackingPlayer();

        GameManager.Instance.LoadData();

        player.spotLight.gameObject.SetActive(true);
    }
}
