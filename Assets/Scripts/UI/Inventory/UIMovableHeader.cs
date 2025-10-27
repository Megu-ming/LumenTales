using UnityEngine;
using UnityEngine.EventSystems;

public class UIMovableHeader : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform target;
    UIManager uiManager;

    private Vector2 beginPos;
    private Vector2 moveBegin;

    public void Init(UIManager uiManager, RectTransform target)
    {
        this.target = target;
        this.uiManager = uiManager;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        beginPos = target.position;
        moveBegin = eventData.position;

        if (target && target.gameObject.TryGetComponent<UIBase>(out UIBase ui))
            uiManager.BringToFront(ui);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 diff = eventData.position - moveBegin;
        target.position = beginPos + diff;
    }

    public void OnClickBtn() => target.gameObject.SetActive(false);
}
