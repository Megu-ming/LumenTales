using Unity.Cinemachine;
using UnityEngine;

public class BattleScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Battle;
    }

    protected override void Start()
    {
        base.Start();
    }
}
