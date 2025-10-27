using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanelUI : MonoBehaviour
{
    public Button newGameBtn; 
    public Button continueBtn;
    public Button exitBtn;

    SlotPanelUI slotPanel;

    public void Init(SlotPanelUI slotPanel)
    {
        this.slotPanel = slotPanel;
    }

    public void OnClickNewGame()
    {
        OpenSlotPanel(true);
    }

    public void OnClickContinue()
    {
        OpenSlotPanel(false);
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
           