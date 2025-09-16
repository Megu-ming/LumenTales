using TMPro;
using UnityEngine;

public class UICharacterInfo : MonoBehaviour
{
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] TextMeshProUGUI levelText, atkText, defText, hpText, speedText, dropText;
    [SerializeField] TextMeshProUGUI strText, dexText, lukText;

    private void OnEnable()
    {
    
    }

    public void Refresh()
    {
        if (playerStatus != null) return;
        if (levelText) levelText.text = $"LEVEL : {playerStatus.Level}";
        if (atkText) atkText.text = $"ATK : {playerStatus.Damage}";
        if(defText) defText.text = $"DEF : {playerStatus.Defense}";
        if (hpText) hpText.text = $"HP : {playerStatus.MaxHealth}";
        if (speedText) speedText.text = $"SPDEED : {playerStatus.MoveSpeed:F1}";
        if (dropText) dropText.text = $"DROP RATE : {playerStatus.DropRate:P1}";

        if (strText) strText.text = $"STR : {playerStatus.Strength}";
        if (dexText) dexText.text = $"DEX : {playerStatus.Agility}";
        if (lukText) lukText.text = $"LUK : {playerStatus.Luck}";
    }
}
