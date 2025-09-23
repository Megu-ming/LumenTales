using UnityEngine;

public class TownScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Town;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
}
