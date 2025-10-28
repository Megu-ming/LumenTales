using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    [SerializeField] UIRoot uiRoot;
    PlayerController pc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        if(uiRoot != null && uiRoot.interactPanel != null)
            uiRoot.interactPanel.SetActive(true);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent += obj.OnInteraction;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        if (uiRoot != null && uiRoot.interactPanel != null)
            uiRoot.interactPanel.SetActive(false);
        if (TryGetComponent<InteractiveObj>(out var obj))
        {
            pc.OnInteractionEvent -= obj.OnInteraction;
        }
    }
}
