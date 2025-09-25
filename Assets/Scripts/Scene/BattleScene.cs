using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Battle;

        GameManager.instance.CurrentScene = this;
    }

    protected override void Start()
    {
        base.Start();

    }
}
