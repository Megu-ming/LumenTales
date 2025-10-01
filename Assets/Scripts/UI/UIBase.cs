using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
public abstract class UIBase : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
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
        UIManager.instance?.Hide();
    }

    public void Open()
    {
        if(isOpen) { UIManager.instance.BringToFront(this); OnFocus(); return; }
        isOpen = true;
        gameObject.SetActive(true);
        UIManager.instance.Push(this);
        OnOpen();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        UIManager.instance.Pop(this);
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

    public virtual void OnPointerDown(PointerEventData e) => UIManager.instance.BringToFront(this);
    public virtual void OnDrag(PointerEventData e) { }
    public virtual void OnPointerUp(PointerEventData e) => UIManager.instance.BringToFront(this);
}
