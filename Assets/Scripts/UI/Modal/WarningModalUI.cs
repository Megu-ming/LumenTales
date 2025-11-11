using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ModalType
{
    InNewGameWarning,
    InContinueWarning,
    InDeleteWarning
}

public class WarningModalUI : MonoBehaviour
{
    [SerializeField] TMP_Text warningText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    int cachedSlotIndex;
    ModalType cachedtype;

    SlotPanelUI slotPanel;

    public void Init(SlotPanelUI slotPanel)
    {
        this.slotPanel = slotPanel;
        gameObject.SetActive(false);
    }

    public void SettingModal(ModalType type, int slot)
    {
        switch(type)
        {
            case ModalType.InNewGameWarning:
                cachedtype = ModalType.InNewGameWarning;
                warningText.text = "이미 저장된 슬롯입니다. 덮어씌우시겠습니까?";
                break;
            case ModalType.InContinueWarning:
                cachedtype = ModalType.InContinueWarning;
                warningText.text = "빈 슬롯입니다. 이 슬롯에서 시작하시겠습니까?";
                break;
            case ModalType.InDeleteWarning:
                cachedtype = ModalType.InDeleteWarning;
                warningText.text = "정말 삭제하시겠습니까?";
                break;
        }

        gameObject.SetActive(true);
        cachedSlotIndex = slot;
    }

    public void OnYesButton()
    {
        gameObject.SetActive(false);

        if(cachedtype == ModalType.InDeleteWarning)
        {
            GameManager.Instance.GetDataManager().DeleteSlot(cachedSlotIndex);
            slotPanel.RefreshSlots();
            return;
        }

        GameManager.Instance.SetSpawnPoint(PlayerSpawnPoint.Default);
        GameManager.Instance.LoadSceneFromNewGame(cachedSlotIndex);
    }

    public void OnNoButton()
    {
        gameObject.SetActive(false);
    }
}
