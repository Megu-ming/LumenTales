using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    [ReadOnly, SerializeField] PlayerController pc;
    [SerializeField] NPCConverstionHandler npcCH;
    [SerializeField] Portal portal;
    [SerializeField] GameObject interactPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        interactPanel.SetActive(true);
        if (npcCH != null)
            pc.OnInteractionEvent += npcCH.ConversationEvent;
        if (portal != null)
            pc.OnInteractionEvent += portal.OnInteraction;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        if (interactPanel) interactPanel.SetActive(false);
        if (npcCH != null)
            pc.OnInteractionEvent -= npcCH.ConversationEvent;
        if (portal != null)
            pc.OnInteractionEvent -= portal.OnInteraction;
    }
}
