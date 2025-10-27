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
        Player.instance.Revive();

        Destroy(gameObject);        
    }

    void OnClickTown()
    {
        if (DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }

        if (GameManager.instance is not null)
            GameManager.instance.LoadScene(SceneType.Town);
    }

    void OnClickMenu()
    {
        if (DataManager.instance is not null)
        {
            DataManager.instance.BackupCurrentSlot();
            DataManager.instance.SaveAll();
        }

        if(GameManager.instance is not null)
            GameManager.instance.LoadScene(SceneType.Menu);
    }
}
