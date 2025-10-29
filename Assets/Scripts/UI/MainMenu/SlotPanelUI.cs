using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotPanelUI : MonoBehaviour
{
    public Button[] slots;
    public Button[] deleteSlots;
    public TMP_Text[] slotTitleLines;

    public Button backBtn;
    WarningModalUI modal;
    DataManager dataManager;

    private bool isFromNewGame;
    public bool IsFromNewGame { get { return isFromNewGame; } set { isFromNewGame = value; } }

    public void Init(WarningModalUI warningModal, DataManager dataManager)
    {
        modal = warningModal;
        this.dataManager = dataManager;

        if(dataManager)
            dataManager.OnSlotsChanged += RefreshSlotsIfOpen;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        dataManager.OnSlotsChanged -= RefreshSlotsIfOpen;
    }

    public void RefreshSlotsIfOpen()
    {
        // 슬롯 패널이 열려있으면 갱신(메인 패널일 때는 생략 가능)
        if (gameObject.activeSelf) RefreshSlots();
        else RefreshTitlesOnly(); // 메인에서도 라벨만 최신화하고 싶다면 호출
    }

    public void RefreshSlots()
    {
        var metas = dataManager.GetSlotMetas();
        for (int i = 0; i < metas.Length; i++)
        {
            var meta = metas[i];

            if (slotTitleLines[i].text != null)
            {
                slotTitleLines[i].text = meta.exists
                    ? $"{meta.TitleLine}" : "(빈 슬롯)";
            }
        }
    }

    void RefreshTitlesOnly()
    {
        if (slotTitleLines == null) return;
        var metas = dataManager.GetSlotMetas();
        for (int i = 0; i < slotTitleLines.Length && i < metas.Length; i++)
        {
            if (slotTitleLines[i] != null)
                slotTitleLines[i].text = metas[i].exists
                    ? $"{metas[i].TitleLine}" : "(빈 슬롯)";
        }
    }

    public void OnClickBack()
    {
        gameObject.SetActive(false);
        isFromNewGame = false;
    }

    public void OnClickSlot(int slot)
    {
        var metas = dataManager.GetSlotMetas();
        if (slot < 0 || slot >= metas.Length) return;

        var meta = metas[slot];

        if (isFromNewGame)
        {
            if (meta.exists)
            {
                // 덮어쓰기 모달 출력
                modal.SettingModal(ModalType.InNewGameWarning, slot);
            }
            else
            {
                GameManager.Instance.SetSpawnPoint(PlayerSpawnPoint.Default);
                GameManager.Instance.LoadSceneFromNewGame(slot);
            }
        }
        else
        {
            if (meta.exists)
            {
                GameManager.Instance.SetSpawnPoint(PlayerSpawnPoint.Default);
                GameManager.Instance.LoadSceneFromContine(slot);
            }
            else
            {
                // 새로 시작할거냐고 묻는 모달
                modal.SettingModal(ModalType.InContinueWarning, slot);
            }
        }
    }

    public void DeleteSlotData(int slot)
    {
        modal.SettingModal(ModalType.InDeleteWarning, slot);
    }
}
