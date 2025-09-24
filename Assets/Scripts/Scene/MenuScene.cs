using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : SceneBase
{
    [SerializeField] GameObject slotPanel;
    [SerializeField] GameObject[] slots;

    protected override void Awake()
    {
        base.Awake();

        sceneType = SceneType.Menu;

        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickNewGame()
    {
        slotPanel.SetActive(true);
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

    public void OnClickSlot(int slot)
    {
        // 해당 슬롯의 데이터가 비었으면 새로 시작

        // 데이터가 있으면 데이터 얻어와서 바로 시작
    }

    public void OnClickBack()
    {
        slotPanel.SetActive(false);
    }
}
