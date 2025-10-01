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
                warningText.text = "The slot already exists. Would you like to overwrite it?";
                break;
            case ModalType.InContinueWarning:
                cachedtype = ModalType.InContinueWarning;
                warningText.text = "The slot is empty. Would you like to start over?";
                break;
            case ModalType.InDeleteWarning:
                cachedtype = ModalType.InDeleteWarning;
                warningText.text = "Are you sure you want to delete it?";
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
