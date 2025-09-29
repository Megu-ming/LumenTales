using UnityEngine;
using UnityEngine.SceneManagement;

public class TownScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Town;

        InitScene();
    }

    protected override void Start()
    {
        base.Start();

        GameManager.instance.InjectData();
    }

    public void InitScene()
    {
        Player.instance?.InventoryController.Init();

    }
}
