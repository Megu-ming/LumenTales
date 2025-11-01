using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingVolume : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text percentageText;

    public void SetVolume()
    {
        percentageText.text = $"{(int)(slider.value * 100)}%";
    }
}
