using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHPBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;

    public void UpdateHpBar(float current, float max)
    {
        float fillAmount = current / max;
        hpBar.fillAmount = fillAmount;
        hpText.text = $"{current:N0} / {max:N0}";
    }
}
