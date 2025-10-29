using UnityEngine;
using UnityEngine.InputSystem;

public class IngameMenu : UIBase
{
    Player player;

    public void Init(UIRoot uiRoot, Player player)
    {
        this.uiRoot = uiRoot;
        this.player = player;
    }

    protected override void OnOpen()
    {
        Time.timeScale = 0f;
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
    protected override void OnClose()
    {
        Time.timeScale = 1f;
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    public void OnClickResume()
    {
        Close();
    }

    public void OnClickMainMenu()
    {
        Close();
        GameManager.Instance.LoadSceneWithSave(SceneType.Menu);
    }

    public void OnClickQuitGame()
    {
        GameManager.Instance.SaveData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
