using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class PointerArea : MonoBehaviour, IPointerDownHandler
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (Player.instance != null)
            Player.instance.PlayerController.OnAttack(eventData);
    }
}
