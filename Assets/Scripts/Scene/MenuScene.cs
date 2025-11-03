using UnityEngine;

public class MenuScene : SceneBase
{
    [SerializeField] MainMenuPanelUI mainMenuUI;
    [SerializeField] SlotPanelUI slotPanelUI;
    [SerializeField] WarningModalUI warningModalUI;
    [SerializeField] SettingPanelUI settingPanelUI;

    protected void Start()
    {
        if(GameManager.Instance)
        {
            GameManager.Instance.IsIngame = false;
            GameManager.Instance.SceneStart();
        }
    }

    public override void Init()
    {
        base.Init();

        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;

        mainMenuUI.Init(slotPanelUI, settingPanelUI);
        slotPanelUI.Init(warningModalUI, GameManager.Instance?.GetDataManager());
        warningModalUI.Init(slotPanelUI);
        settingPanelUI.Init();

        SoundManager.PlayBGM(BgmType.Menu);
    }
}