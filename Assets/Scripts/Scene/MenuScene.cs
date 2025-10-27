using UnityEngine;

public class MenuScene : SceneBase
{
    [SerializeField] MainMenuPanelUI mainMenuUI;
    [SerializeField] SlotPanelUI slotPanelUI;
    [SerializeField] WarningModalUI warningModalUI;

    public override void Init()
    {
        base.Init();

        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;

        mainMenuUI.Init(slotPanelUI);
        slotPanelUI.Init(warningModalUI, GameManager.instance.GetDataManager());
        warningModalUI.Init(slotPanelUI);
    }
}