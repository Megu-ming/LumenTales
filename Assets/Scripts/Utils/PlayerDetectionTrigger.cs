using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    PlayerController pc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;
        if(UIManager.instance != null && UIManager.instance.interactPanel != null)
            UIManager.instance.interactPanel.SetActive(true);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent += obj.OnInteraction;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        if (UIManager.instance != null && UIManager.instance.interactPanel != null)
            UIManager.instance.interactPanel.SetActive(false);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent -= obj.OnInteraction;
        }
    }
}
