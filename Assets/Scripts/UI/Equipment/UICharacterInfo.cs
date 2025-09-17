using System;
using TMPro;
using UnityEngine;

public class UICharacterInfo : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] TextMeshProUGUI levelText, atkText, defText, hpText, speedText, dropText;
    [SerializeField] TextMeshProUGUI strText, dexText, lukText, spText;

    private void Awake()
    {
        if(playerStatus == null)
        {
            playerStatus = GameManager.Instance.GetStatus();
        }
        Refresh();

        GameManager.Instance.expChanged += Refresh;
    }

    private void OnDestroy()
    {
        GameManager.Instance.expChanged -= Refresh;
    }

    public void OnAtkButton()
    {
        if (playerStatus == null) return;
        if(GameManager.Instance.GetStatusPoint() <= 0) return;
        playerStatus.Strength++;
        GameManager.Instance.UseStatusPoint();
        Refresh();
    }
    public void OnDexButton()
    {
        if (playerStatus == null) return;
        if (GameManager.Instance.GetStatusPoint() <= 0) return;
        playerStatus.Agility++;
        GameManager.Instance.UseStatusPoint();
        Refresh();
    }

    public void OnLuckButton()
    {
        if (playerStatus == null) return;
        if (GameManager.Instance.GetStatusPoint() <= 0) return;
        playerStatus.Luck++;
        GameManager.Instance.UseStatusPoint();
        Refresh();
    }

    public void Refresh()
    {
        if (playerStatus == null) return;
        if (levelText) levelText.text = $"{playerStatus.Level}";
        if (atkText) atkText.text = $"{playerStatus.Attack}";
        if(defText) defText.text = $"{playerStatus.Defense}";
        if (hpText) hpText.text = $"{playerStatus.MaxHealth}";
        if (speedText) speedText.text = $"{playerStatus.MoveSpeed:F1}";
        if (dropText) dropText.text = $"{playerStatus.DropRate:P1}";

        if (strText) strText.text = $"{playerStatus.Strength}";
        if (dexText) dexText.text = $"{playerStatus.Agility}";
        if (lukText) lukText.text = $"{playerStatus.Luck}";

        if (spText) spText.text = $"{GameManager.Instance.GetStatusPoint()}";
    }
}
