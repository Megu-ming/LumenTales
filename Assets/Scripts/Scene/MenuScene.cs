using UnityEngine;

public class MenuScene : SceneBase
{
    [SerializeField] MainMenuPanelUI mainMenuUI;
    [SerializeField] SlotPanelUI slotPanelUI;
    [SerializeField] WarningModalUI warningModalUI;

    protected void Start()
    {
        if(GameManager.Instance)    
            GameManager.Instance.SceneStart();
    }

    public override void Init()
    {
        base.Init();

        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;

        mainMenuUI.Init(slotPanelUI);
        slotPanelUI.Init(warningModalUI, GameManager.Instance?.GetDataManager());
        warningModalUI.Init(slotPanelUI);
    }
}