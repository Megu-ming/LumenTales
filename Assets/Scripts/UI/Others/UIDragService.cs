using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragService : MonoBehaviour
{
    public static UIDragService I { get; private set; }

    [Header("Setup")]
    [SerializeField] Canvas rootCanvas;
    [SerializeField] GraphicRaycaster uiRaycaster;
    [SerializeField] GameObject imageDummy;      // 더미 아이콘

    // 상태
    public UISlotBase From { get; private set; }
    public ItemData Payload { get; private set; }

    PointerEventData ped;
    readonly List<RaycastResult> rr = new();

    void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;

        if (!rootCanvas) rootCanvas = GetComponentInParent<Canvas>();
        if (!uiRaycaster) uiRaycaster = rootCanvas.GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(EventSystem.current);
        if (imageDummy) imageDummy.GetComponent<Image>().enabled = false;
    }

    public void Begin(UISlotBase from, Sprite icon, Vector2 screenPos)
    {
        From = from;
        Payload = from.PeekItem();
        if (!Payload) return;

        // 더미 아이콘 표시
        if (imageDummy)
        {
            var img = imageDummy.GetComponent<Image>();
            img.sprite = icon;
            img.enabled = true;
            imageDummy.transform.position = screenPos;
            imageDummy.transform.SetAsLastSibling();
        }
    }

    public void Move(Vector2 screenPos)
    {
        if (!From) return;
        if (imageDummy) imageDummy.transform.position = screenPos;
    }

    public void End(Vector2 screenPos, InventoryController inventory)
    {
        if (!From) { HideDummy(); return; }

        ped.position = screenPos;
        rr.Clear();
        uiRaycaster.Raycast(ped, rr);

        UISlotBase to = null;
        for (int i = 0; i < rr.Count; i++)
        {
            to = rr[i].gameObject.GetComponent<UISlotBase>();
            if (to != null) break;
        }

        if (to != null && Payload != null && to.CanAccept(Payload))
        {
            // 모델에 단일 API로 위임
            inventory?.TryTransfer(From.Address, to.Address);
        }

        From = null;
        Payload = null;
        HideDummy();
    }

    void HideDummy()
    {
        if (imageDummy)
        {
            var img = imageDummy.GetComponent<Image>();
            if (img) img.enabled = false;
        }
    }
}
