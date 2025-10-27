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
    public From owner;
    public event Action<bool, int> HandleYesButton;

    public void Init(UIManager uiManager)
    {
        this.uiManager = uiManager;

        yesBtn.onClick.AddListener(OnClickYesButton);
        noBtn.onClick.AddListener(OnClickNoButton);
        gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        if (owner is From.Store)
        {
            HandleYesButton -= uiManager.storeUI.GetChoicedSlot().TryBuyItem;
            HandleYesButton += uiManager.storeUI.GetChoicedSlot().TryBuyItem;
        }
        if (owner is From.Inventory)
        {
            HandleYesButton -= uiManager.inventoryUI.TrySellItem;
            HandleYesButton += uiManager.inventoryUI.TrySellItem;
        }
    }

    override protected void OnClose()
    {
        if (owner is From.Store)
        {
            HandleYesButton -= uiManager.storeUI.GetChoicedSlot().TryBuyItem;
        }
        if (owner is From.Inventory)
        {
            HandleYesButton -= uiManager.inventoryUI.TrySellItem;
        }
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
