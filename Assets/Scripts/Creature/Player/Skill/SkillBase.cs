using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillBase : MonoBehaviour
{
    protected Player player;
    [SerializeField] float coolTime;
    private float coolDownTimer;

    // Update is called once per frame
    void Update()
    {
        if (coolDownTimer > 0f)
            coolDownTimer -= Time.deltaTime;
    }

    public void UseSkillInputAction(InputAction.CallbackContext context)
    {
        if(coolDownTimer <= 0f)
        {
            coolDownTimer += coolTime;
            UseSkill();
        }
    }

    public abstract void UseSkill();

    public virtual void Init(Player player)
    {
        this.player = player;
    }
}
