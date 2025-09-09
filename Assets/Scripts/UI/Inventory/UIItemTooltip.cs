using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemTooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI atkValueText;
    [SerializeField] TextMeshProUGUI descriptionText;

    public void SetupTooltip(string name, string type, string des, int atk = 0)
    {
        nameText.text = name;
        descriptionText.text = des;

        if (atk == 0)
        {
            atkText.gameObject.SetActive(false);
            atkValueText.gameObject.SetActive(false);
        }
        else
        {
            atkText.gameObject.SetActive(true);
            atkValueText.gameObject.SetActive(true);
            atkValueText.text = atk.ToString();
        }
    }
}
