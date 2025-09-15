using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
public abstract class UIBase : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("UI Base Option")]
    public bool closeOnEsc = true;

    protected Canvas canvas;
    protected CanvasGroup canvasGroup;
    bool isOpen;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        if(!TryGetComponent<CanvasGroup>(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        HideImmediate();
    }

    protected virtual void OnDisable()
    {
        TooltipService.I?.Hide();
    }

    public void Open()
    {
        if(isOpen) { UIStackManager.Instance.BringToFront(this); OnFocus(); return; }
        isOpen = true;
        gameObject.SetActive(true);
        UIStackManager.Instance.Push(this);
        Show();
        OnOpen();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        UIStackManager.Instance.Pop(this);
        OnClose();
        Hide();
    }

    public void Toggle()
    {
        if(gameObject.activeSelf) Close(); 
        else Open(); 
    }

    public void SetSortingOrder(int order) => canvas.sortingOrder = order;

    public virtual void OnFocus() => transform.SetAsLastSibling();

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    protected virtual void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    protected virtual void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        gameObject.SetActive(false);
    }

    void HideImmediate()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData e) => UIStackManager.Instance.BringToFront(this);
    public void OnDrag(PointerEventData e) => UIStackManager.Instance.BringToFront(this);
}