using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [SerializeField] Button reviveButton;
    [SerializeField] Button townButton;
    [SerializeField] Button mainMenuButton;

    private void Awake()
    {
        reviveButton.onClick.AddListener(OnClickRevive);
        townButton.onClick.AddListener(OnClickTown);
        mainMenuButton.onClick.AddListener(OnClickMenu);
    }

    void OnClickRevive()
    {
        Debug.Log("Revive!");
    }

    void OnClickTown()
    {
        Debug.Log("Respawn To Town!");
    }

    void OnClickMenu()
    {
        Debug.Log("Go To Mainmenu!");
    }
}
