using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public GameObject hpBar;

    public float curHp;
    public float maxHp;
    public Image hpImg;

    private void Awake()
    {
        hpImg = hpBar.GetComponent<Image>();
    }

    private void Update()
    {
        hpImg.fillAmount = Utils.Percent(curHp, maxHp);
    }
}
