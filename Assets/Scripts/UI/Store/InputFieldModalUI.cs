using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldModalUI : UIBase
{
    [SerializeField] TMP_InputField amount;
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;

    public int maxAmount;

    public event Action<bool, int> HandleYesButton;

    private void Start()
    {
        yesBtn.onClick.AddListener(OnClickYesButton);
        noBtn.onClick.AddListener(OnClickNoButton);
        gameObject.SetActive(false);
    }

    void OnClickYesButton()
    {
        // 숫자입력이 잘못됐으면 아무일도 없음
        if (int.TryParse(amount.text, out var value) is false)
            return;
        HandleYesButton?.Invoke(true, value);
        Close();
    }

    void OnClickNoButton()
    {
        // 그냥 종료
        Close();
    }

    public void OnEndEdit()
    {
        if (maxAmount > 0 && int.Parse(amount.text) > maxAmount)
            amount.text = maxAmount.ToString();
    }
}
