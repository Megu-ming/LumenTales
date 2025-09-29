using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : SceneBase
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject slotPanel;


    [Header("Main Buttons")]
    [SerializeField] Button newGameBtn;
    [SerializeField] Button continueBtn;
    [SerializeField] Button settingsBtn;

    [SerializeField] Button backBtn;

    [Header("Slots (0~4)")]
    [SerializeField] Button[]   slots;
    [SerializeField] Button[]   deleteSlots;
    [SerializeField] TMP_Text[] slotTitleLines;

    bool isFromNewGame = false;

    protected override void Awake()
    {
        base.Awake();
        // 기본 씬 설정
        sceneType = SceneType.Menu;
        Cursor.lockState = CursorLockMode.None;
        slotPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if(DataManager.instance)
            DataManager.instance.OnSlotsChanged += RefreshSlotsIfOpen;
    }

    private void OnDisable()
    {
        if(DataManager.instance)
            DataManager.instance.OnSlotsChanged -= RefreshSlotsIfOpen;
    }

    void RefreshSlotsIfOpen()
    {
        // 슬롯 패널이 열려있으면 갱신(메인 패널일 때는 생략 가능)
        if (slotPanel && slotPanel.activeSelf) RefreshSlots();
        else RefreshTitlesOnly(); // 메인에서도 라벨만 최신화하고 싶다면 호출
    }

    void RefreshSlots()
    {
        if (slotPanel == null) return;
        var metas = DataManager.instance.GetSlotMetas();
        for(int i=0; i< metas.Length; i++)
        {
            var meta = metas[i];

            if (slotTitleLines[i].text!=null)
            {
                slotTitleLines[i].text = meta.exists
                    ? $"{meta.TitleLine}" : "(Empty Slot)";
            }
        }
    }

    void RefreshTitlesOnly()
    {
        if (slotTitleLines == null) return;
        var metas = DataManager.instance.GetSlotMetas();
        for (int i = 0; i < slotTitleLines.Length && i < metas.Length; i++)
        {
            if (slotTitleLines[i] != null)
                slotTitleLines[i].text = metas[i].exists
                    ? $"{metas[i].TitleLine}" : "(Empty Slot)";
        }
    }

    #region 클릭 이벤트
    public void OnClickNewGame()
    {
        isFromNewGame = true;
        OpenSlotPanel();
    }

    public void OnClickContinue()
    {
        isFromNewGame = false;
        OpenSlotPanel();
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

    public void OnClickBack()
    {
        slotPanel.SetActive(false);
        isFromNewGame = false;
    }

    public void OnClickSlot(int slot)
    {
        var metas = DataManager.instance.GetSlotMetas();
        if (slot < 0 || slot >= metas.Length) return;

        var meta = metas[slot];

        if (isFromNewGame)
        {
            if (meta.exists)
            {
                // 덮어쓰기 모달 출력

            }
            else
            { 
                DataManager.instance.NewGameAtSlot(slot);
                GameManager.instance.LoadScene(SceneType.Town);
            }
        }
        else
        {
            if (meta.exists)
            {
                DataManager.instance.ContinueAtSlot(slot);
                GameManager.instance.LoadScene(SceneType.Town);
            }
            else
            {
                // 빈 슬롯을 눌렀으니 슬롯 진동 효과나 뭐 그런거?
            }
        }
    }

    public void DeleteSlotData(int slot)
    {
        DataManager.instance.DeleteSlot(slot);

        RefreshSlots();
    }

    void OpenSlotPanel()
    {
        if (!slotPanel) return;
        slotPanel.SetActive(true);
        RefreshSlots();
    }
    #endregion
}
