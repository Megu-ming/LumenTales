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

    void Start()
    {
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
            DataManager.instance.DeleteSlot(cachedSlotIndex);
            UIManager.instance.slotPanel.RefreshSlots();
            return;
        }

        DataManager.instance.NewGameAtSlot(cachedSlotIndex);
        GameManager.instance.LoadScene(SceneType.Town);
    }

    public void OnNoButton()
    {
        gameObject.SetActive(false);
    }
}
