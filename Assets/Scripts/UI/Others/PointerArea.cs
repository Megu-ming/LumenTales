using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class PointerArea : MonoBehaviour, IPointerDownHandler
{
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Player.instance.PlayerController.OnAttack(eventData);
    }
}
