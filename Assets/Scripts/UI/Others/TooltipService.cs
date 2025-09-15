using UnityEngine;

public class TooltipService : MonoBehaviour
{
    public static TooltipService I { get; private set; }

    [Header("Setup")]
    [SerializeField] GameObject tooltipPrefab;
    [SerializeField] Canvas rootCanvas;

    UIItemTooltip tooltip;
    RectTransform tooltipRect;
    RectTransform canvasRect;
    Camera uiCam;

    private void Awake()
    {
        if (I && I != this) { Destroy(gameObject); return; }
        I = this;

        if(!rootCanvas) rootCanvas = GetComponentInParent<Canvas>();
        canvasRect = rootCanvas.GetComponent<RectTransform>();
        uiCam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        if(!tooltip&&tooltipPrefab)
        {
            tooltip = Instantiate(tooltipPrefab, rootCanvas.transform).GetComponent<UIItemTooltip>();
            tooltipRect = tooltip.GetComponent<RectTransform>();
            tooltip.gameObject.SetActive(false);
        }
    }

    public void Show(string name, string desc, int price, Vector2 screenPos, int atk = 0, bool isAtk = false, bool isDef = false)
    {
        if(!tooltip) return;

        tooltip.SetupTooltip(name, desc, price, atk, isAtk, isDef);

        tooltip.transform.position = screenPos;

        float pivotX = tooltipRect.anchoredPosition.x + tooltipRect.sizeDelta.x > canvasRect.sizeDelta.x ? 1f : 0f; // anchor 11
        float pivotY = tooltipRect.anchoredPosition.y - tooltipRect.sizeDelta.y < -canvasRect.sizeDelta.y ? 0f : 1f; // anchor 00

        tooltipRect.pivot = new Vector2(pivotX, pivotY);

        tooltip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if(tooltip) tooltip.gameObject.SetActive(false);
    }
}
