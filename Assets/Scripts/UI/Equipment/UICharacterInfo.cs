using System;
using TMPro;
using UnityEngine;

public class UICharacterInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText, atkText, defText, hpText, speedText, dropText;
    [SerializeField] TextMeshProUGUI strText, dexText, spText;
    Player player;

    public void Init(Player player)
    {
        this.player = player;
        Refresh();

        CharacterEvents.infoUIRefresh += Refresh;
    }

    private void OnDestroy()
    {
        CharacterEvents.infoUIRefresh -= Refresh;
    }

    public void OnStrButton()
    {
        if(GameManager.Instance.CurrentScene.StatusPoint <= 0) return;
        player.Status.SpAddedStr++;
        GameManager.Instance.CurrentScene.UseStatusPoint();
        Refresh();
    }
    public void OnDexButton()
    {
        if (GameManager.Instance.CurrentScene.StatusPoint <= 0) return;
        player.Status.SpAddedAgi++;
        GameManager.Instance.CurrentScene.UseStatusPoint();
        Refresh();
    }

    public void Refresh()
    {
        var playerStatus = player.Status;
        if (playerStatus == null) return;
        if (levelText)  levelText.text = $"{playerStatus.Level}";
        if (atkText)    atkText.text = $"{(int)playerStatus.FinalAttack}";
        if (hpText)     hpText.text = $"{playerStatus.FinalMaxHealth}";

        if (strText)    strText.text = $"{playerStatus.Strength}";
        if (dexText)    dexText.text = $"{playerStatus.Agility}";

        if (spText) spText.text = $"{GameManager.Instance?.CurrentScene.StatusPoint}";
    }
}
