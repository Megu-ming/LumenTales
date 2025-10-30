using UnityEngine;
using UnityEngine.InputSystem;
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

        if(player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Dead");
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            var playerInput = player.GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    void OnClickRevive()
    {
        player.Revive();

        Destroy(gameObject);        
    }

    void OnClickTown()
    {
        player.Revive();
        GameManager.Instance.SetSpawnPoint(PlayerSpawnPoint.Default);
        GameManager.Instance.LoadSceneWithSave(SceneType.Town);
    }

    void OnClickMenu()
    {
        GameManager.Instance.SetSpawnPoint(PlayerSpawnPoint.Default);
        GameManager.Instance.LoadSceneWithSave(SceneType.Menu);
    }
}
