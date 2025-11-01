using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuPanelUI : MonoBehaviour
{
    [SerializeField] Button newGameBtn; 
    [SerializeField] Button continueBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Button exitBtn;

    SlotPanelUI slotPanel;
    SettingPanelUI setting;

    public void Init(SlotPanelUI slotPanel, SettingPanelUI settingPanel)
    {
        this.slotPanel = slotPanel;
        setting = settingPanel;
    }

    public void OnClickNewGame()
    {
        OpenSlotPanel(true);
    }

    public void OnClickContinue()
    {
        OpenSlotPanel(false);
    }

    public void OnClickSetting()
    {
        OpenSetting();
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;    // �����Ϳ��� ����
#else
        Application.Quit();                     // ��� �� ����
#endif
    }

    void OpenSlotPanel(bool value)
    {
        if(slotPanel != null)
        {
            slotPanel.IsFromNewGame = value;
            slotPanel.gameObject.SetActive(true);
        }
        slotPanel.RefreshSlots();
    }
    void OpenSetting()
    {
        setting.gameObject.SetActive(true);
    }
}          
           