using UnityEngine;

public class PlayerDetectionTrigger : MonoBehaviour
{
    PlayerController pc;
    NPCConverstionHandler npcCH;
    Portal portal;
    Store store;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;
        if(UIManager.instance != null && UIManager.instance.interactPanel != null)
            UIManager.instance.interactPanel.SetActive(true);
        if (TryGetComponent<NPCConverstionHandler>(out npcCH))
        {
            pc.OnInteractionEvent += npcCH.ConversationEvent;
            return;
        }
        if (TryGetComponent<Portal>(out portal))
        {
            pc.OnInteractionEvent += portal.OnInteraction;
            return;
        }
        if(TryGetComponent<Store>(out store))
        {
            pc.OnInteractionEvent += store.OpenStoreUI;
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out pc)) return;

        if (UIManager.instance != null && UIManager.instance.interactPanel != null)
            UIManager.instance.interactPanel.SetActive(false);
        if (npcCH != null)
            pc.OnInteractionEvent -= npcCH.ConversationEvent;
        if (portal != null)
            pc.OnInteractionEvent -= portal.OnInteraction;
        if (store != null)
            pc.OnInteractionEvent -= store.OpenStoreUI;
    }
}
