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
    [SerializeField] TextMeshProUGUI priceValueText;

    public void SetupTooltip(string name, string des, int price, int eqVal =0, bool isAtkVal = false, bool isDefVal = false)
    {
        nameText.text = name;
        descriptionText.text = des;

        if (!isAtkVal&&!isDefVal)
        {
            atkText.gameObject.SetActive(false);
            atkValueText.gameObject.SetActive(false);
        }
        else if(isDefVal)
        {
            atkText.gameObject.SetActive(true);
            atkText.text = "DEF";
            atkValueText.gameObject.SetActive(true);
            atkValueText.text = eqVal.ToString();
        }
        else if (isAtkVal)
        {
            atkText.gameObject.SetActive(true);
            atkText.text = "ATK";
            atkValueText.gameObject.SetActive(true);
            atkValueText.text = eqVal.ToString();
        }

        priceValueText.text = price.ToString();
    }
}
