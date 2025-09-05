using UnityEngine;
using UnityEngine.EventSystems;

public class UIMovableHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform target;

    private Vector2 beginPos;
    private Vector2 moveBegin;

    private void Awake()
    {
        if (target == null)
            target = transform.parent.GetComponent<RectTransform>();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPos = target.position;
        moveBegin = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 diff = eventData.position - moveBegin;
        target.position = beginPos + diff;
    }

    public void OnClickBtn() => target.gameObject.SetActive(false);


}
