using UnityEngine;
using UnityEngine.SceneManagement;

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
}
