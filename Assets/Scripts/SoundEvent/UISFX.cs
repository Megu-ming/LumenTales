using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISFX : MonoBehaviour, IPointerEnterHandler
{
    [Header("sfx 설정")]
    [SerializeField] SfxType hoverType = SfxType.UI_Hover;
    [SerializeField] SfxType clickType = SfxType.UI_Click;


    [Header("컴포넌트 참조")]
    [SerializeField] Button button;

    private void Awake()
    {
        if(button == null)
        {
            button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.PlayRandomSFX(hoverType);
    }

    public void OnButtonClick()
    {
        SoundManager.PlayRandomSFX(clickType);
    }
}
