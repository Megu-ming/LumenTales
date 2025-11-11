using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingVolume : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text percentageText;
    [SerializeField] EAudioMixerType audioMixerType;

    private void OnEnable()
    {
        UpdatePercent();
    }

    public void SetVolume()
    {
        SoundManager.instance.SetAudioVolume(audioMixerType, slider.value);
        UpdatePercent();
    }

    public void UpdatePercent()
    {
        float volumeValue = SoundManager.instance.GetAudioVolume(audioMixerType);
        // 퍼센티지 수정
        percentageText.text = $"{volumeValue * 100:N0}%";
        // 슬라이더 수정
        slider.value = volumeValue;
    }
}
