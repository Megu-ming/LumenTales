using UnityEngine;
using UnityEngine.InputSystem;

public class IngameMenu : UIBase
{
    Player player;

    public void Init(Player player)
    {
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
        GameManager.instance.LoadSceneWithSave(SceneType.Menu);
    }

    public void OnClickQuitGame()
    {
        GameManager.instance.SaveData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
