using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    public void OnClickResume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnClickOption()
    {

    }

    public void OnClickMainMenu()
    {
        DataManager.instance.BackupCurrentSlot();
        DataManager.instance.SaveAll();
        Time.timeScale = 1;
        GameManager.instance.LoadScene(SceneType.Menu);
    }

    public void OnClickQuitGame()
    {
        DataManager.instance.BackupCurrentSlot();
        DataManager.instance.SaveAll();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
