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
            playerStatus = Player.instance?.Status;
        }
        Refresh();

        CharacterEvents.infoUIRefresh += Refresh;
    }

    private void OnDestroy()
    {
        CharacterEvents.infoUIRefresh -= Refresh;
    }

    public void OnStrButton()
    {
        if (playerStatus == null) return;
        if(GameManager.instance.CurrentScene.StatusPoint <= 0) return;
        playerStatus.SpAddedStr++;
        GameManager.instance.CurrentScene.UseStatusPoint();
        Refresh();
    }
    public void OnDexButton()
    {
        if (playerStatus == null) return;
        if (GameManager.instance.CurrentScene.StatusPoint <= 0) return;
        playerStatus.SpAddedAgi++;
        GameManager.instance.CurrentScene.UseStatusPoint();
        Refresh();
    }

    public void OnLuckButton()
    {
        if (playerStatus == null) return;
        if (GameManager.instance.CurrentScene.StatusPoint <= 0) return;
        playerStatus.SpAddedLuk++;
        GameManager.instance.CurrentScene.UseStatusPoint();
        Refresh();
    }

    public void Refresh()
    {
        if (playerStatus == null) return;
        if (levelText)  levelText.text = $"{playerStatus.Level}";
        if (atkText)    atkText.text = $"{(int)playerStatus.FinalAtkDamage}";
        if (defText)    defText.text = $"{playerStatus.BaseDefense}";
        if (hpText)     hpText.text = $"{playerStatus.FinalMaxHealth}";
        if (speedText)  speedText.text = $"{playerStatus.MoveSpeed:F1}";
        if (dropText)   dropText.text = $"{playerStatus.DropRate:P1}";

        if (strText)    strText.text = $"{playerStatus.Strength}";
        if (dexText)    dexText.text = $"{playerStatus.Agility}";
        if (lukText)    lukText.text = $"{playerStatus.Luck}";

        if (spText) spText.text = $"{GameManager.instance.CurrentScene.StatusPoint}";
    }
}
