using UnityEngine;
using UnityEngine.InputSystem;

public class IngameMenu : UIBase
{
    protected override void OnOpen()
    {
        Time.timeScale = 0f;
        if (Player.instance != null)
        {
            var playerInput = Player.instance.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
    protected override void OnClose()
    {
        Time.timeScale = 1f;
        if (Player.instance != null)
        {
            var playerInput = Player.instance.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void OnClickResume()
    {
        Close();
    }

    public void OnClickMainMenu()
    {
        if(DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }        
        Close();
        GameManager.instance.LoadScene(SceneType.Menu);
    }

    public void OnClickQuitGame()
    {
        if(DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
