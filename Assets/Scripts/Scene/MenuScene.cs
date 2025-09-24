using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : SceneBase
{
    [SerializeField] GameObject slotPanel;
    [SerializeField] GameObject[] slots;

    bool isFromNewGame = false;

    protected override void Awake()
    {
        base.Awake();
        // 기본 씬 설정
        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;
        slotPanel.SetActive(false);

        // 슬롯별 데이터 로드
        for (int i = 0; i < slots.Length; i++)
        {
            if (!DataManager.instance.LoadGameData(i))
            {
                var tmp = slots[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null) tmp.text = "(empty slot)";
            }
        }
    }

    public void OnClickNewGame()
    {
        slotPanel.SetActive(true);
        isFromNewGame = true;
    }

    public void OnClickContinue()
    {
        slotPanel.SetActive(true);
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
        // newGame을 눌러서 들어온 상태
        if(isFromNewGame)
        {
            // 해당 슬롯의 데이터가 비었으면 새로 시작
            GameManager.instance.StartGame(slot);
            // 데이터가 있으면 데이터 덮어씌울거냐고 물어보기
        }
        else // continue를 눌렀을 때 들어온 상태
        {
            // 해당 슬롯이 비었으면 무시

            // 데이터가 있으면 데이터 가져와서 시작
            GameManager.instance.StartGame(slot);
        }
    }

    public void OnClickBack()
    {
        slotPanel.SetActive(false);
        isFromNewGame = false;
    }
}
