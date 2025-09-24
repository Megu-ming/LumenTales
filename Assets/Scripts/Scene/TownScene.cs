using UnityEngine;

public class TownScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Town;

        GameManager.instance.CurrentScene = this;
    }

    protected override void Start()
    {
        base.Start();
    }
}
