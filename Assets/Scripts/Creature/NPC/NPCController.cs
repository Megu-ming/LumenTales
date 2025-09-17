using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] GameObject interactPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (interactPanel == null) return;
        if(collision.gameObject.tag == "Player")
            interactPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (interactPanel == null) return;
        if (collision.gameObject.tag == "Player")
            interactPanel.SetActive(false);
    }
}