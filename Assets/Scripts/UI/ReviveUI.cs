using UnityEngine;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    [SerializeField] Button reviveButton;
    [SerializeField] Button townButton;
    [SerializeField] Button mainMenuButton;
    Player player;

    public void Init(Player player)
    {
        this.player = player;

        reviveButton.onClick.AddListener(OnClickRevive);
        townButton.onClick.AddListener(OnClickTown);
        mainMenuButton.onClick.AddListener(OnClickMenu);
    }

    void OnClickRevive()
    {
        player.Revive();

        Destroy(gameObject);        
    }

    void OnClickTown()
    {
        GameManager.instance.LoadSceneWithSave(SceneType.Town);
    }

    void OnClickMenu()
    {
        GameManager.instance.LoadSceneWithSave(SceneType.Menu);
    }
}
