using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
public abstract class UIBase : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    protected UIRoot uiRoot;

    [Header("UI Base Option")]
    public bool closeOnEsc = true;

    protected Canvas canvas;
    bool isOpen;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
    }

    protected virtual void OnDisable()
    {
        if(uiRoot is not null)
           uiRoot.Hide();
    }

    public void Open()
    {
        if(isOpen) { uiRoot.BringToFront(this); OnFocus(); return; }
        isOpen = true;
        gameObject.SetActive(true);
        uiRoot.Push(this);
        OnOpen();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        uiRoot.Pop(this);
        OnClose();
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if(gameObject.activeSelf) Close(); 
        else Open(); 
    }

    public void SetSortingOrder(int order) { if (canvas is not null) canvas.sortingOrder = order; }

    public virtual void OnFocus() => transform.SetAsLastSibling();

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    public virtual void OnPointerDown(PointerEventData e) => uiRoot.BringToFront(this);
    public virtual void OnDrag(PointerEventData e) { }
    public virtual void OnPointerUp(PointerEventData e) => uiRoot.BringToFront(this);
}
