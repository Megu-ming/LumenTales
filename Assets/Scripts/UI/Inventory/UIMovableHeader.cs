using UnityEngine;
using UnityEngine.EventSystems;

public class UIMovableHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    RectTransform target;
    UIRoot uiRoot;

    private Vector2 beginPos;
    private Vector2 moveBegin;

    public void Init(UIRoot uiRoot, RectTransform target)
    {
        this.target = target;
        this.uiRoot = uiRoot;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        beginPos = target.position;
        moveBegin = eventData.position;

        if (target && target.gameObject.TryGetComponent<UIBase>(out UIBase ui))
            uiRoot.BringToFront(ui);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 diff = eventData.position - moveBegin;
        target.position = beginPos + diff;
    }

    public void OnClickBtn() => target.gameObject.SetActive(false);
}
