using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : SceneBase
{
    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Menu;

        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickContinue()
    {

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
}
