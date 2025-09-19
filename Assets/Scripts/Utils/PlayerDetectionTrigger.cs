using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    [SerializeField] NPCConverstionHandler npcCH;
    [SerializeField] GameObject interactPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        interactPanel.SetActive(true);
        pc.OnInteractionEvent += npcCH.ConversationEvent;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        interactPanel.SetActive(false);
        pc.OnInteractionEvent -= npcCH.ConversationEvent;
    }
}
