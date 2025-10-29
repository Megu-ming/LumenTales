using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHPBar : MonoBehaviour
{
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        float fillAmount = currentHp / maxHp;
        hpBar.fillAmount = fillAmount;
        hpText.text = $"{currentHp} / {maxHp}";
    }
}
