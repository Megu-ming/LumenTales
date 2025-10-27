using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    PlayerController pc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;
        UIManager ui = GameManager.instance.GetUIManager();
        if(ui != null && ui.interactPanel != null)
            ui.interactPanel.SetActive(true);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent += obj.OnInteraction;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;
        UIManager ui = GameManager.instance.GetUIManager();

        if (ui != null && ui.interactPanel != null)
            ui.interactPanel.SetActive(false);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent -= obj.OnInteraction;
        }
    }
}
