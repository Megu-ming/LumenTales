using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] GameObject interactPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interactPanel == null) return;
        
        if (collision.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            interactPanel.SetActive(true);
            pc.canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactPanel == null) return;
        if (collision.TryGetComponent<PlayerController>(out PlayerController pc))
        {
            interactPanel.SetActive(false);
            pc.canInteract = false;
        }
    }
}