using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingVolume : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text percentageText;
    [SerializeField] EAudioMixerType audioMixerType;

    public void SetVolume()
    {
        percentageText.text = $"{(int)(slider.value * 100)}%";
        SoundManager.instance.SetAudioVolume(audioMixerType, slider.value);
    }
}
