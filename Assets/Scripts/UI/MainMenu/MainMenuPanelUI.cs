using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanelUI : MonoBehaviour
{
    public Button newGameBtn; 
    public Button continueBtn;
    public Button settingsBtn;
    public Button exitBtn;

    SlotPanelUI slotPanel;

    public void Init()
    {
        slotPanel = UIManager.instance.slotPanel;
    }

    public void OnClickNewGame()
    {
        OpenSlotPanel(true);
    }

    public void OnClickContinue()
    {
        OpenSlotPanel(false);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;    // 에디터에서 종료
#else
        Application.Quit();                     // 빌드 시 종료
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
}          
           