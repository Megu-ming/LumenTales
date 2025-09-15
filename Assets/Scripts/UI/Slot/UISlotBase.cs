using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UISlotBase : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Common")]
    [SerializeField] protected Image iconImage;
    [SerializeField] protected InventoryController inventory;

    protected UIBase rootUI;

    protected virtual void Awake()
    {
        if (!rootUI) rootUI = GetComponentInParent<UIBase>();
    }

    // ---- 구현 필수 ----
    public abstract SlotAddress Address { get; }
    public abstract bool HasItem { get; }
    public abstract ItemData PeekItem();
    public abstract bool CanAccept(ItemData data);
    public abstract Sprite GetIcon();
    public abstract void SetInventoryController(InventoryController ic);

    // ---- 포인터/드래그 공통 ----
    public virtual void OnPointerEnter(PointerEventData eventData) { }
    public virtual void OnPointerExit(PointerEventData eventData) { }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        UIStackManager.Instance.BringToFront(rootUI);
        if (!HasItem) return;

        var icon = GetIcon();
        if (icon == null) return;

        UIDragService.I?.Begin(this, icon, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIDragService.I?.Move(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIDragService.I?.End(eventData.position, inventory);
    }
}
