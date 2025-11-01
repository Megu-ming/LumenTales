using UnityEngine;
using UnityEngine.UI;

public class SettingPanelUI : MonoBehaviour
{
    [SerializeField] Button backBtn;

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void OnBackBtnClicked()
    {
        gameObject.SetActive(false);
    }
}
