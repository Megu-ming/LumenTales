using UnityEngine;
using UnityEngine.UI;

public class DashView : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Animator animator;

    public void UpdateCooltime(float cooldown, float coolTime)
    {
        float fill = cooldown / coolTime;
        image.fillAmount = fill;

        if(fill >= 1f)
        {
            SetCooldownOn(false);
        }
    }

    public void SetCooldownOn(bool val)
    {
        animator.SetBool(AnimationStrings.cooldownOn, val);
    }
}
