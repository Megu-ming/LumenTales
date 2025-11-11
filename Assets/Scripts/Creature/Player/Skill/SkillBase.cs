using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillBase : MonoBehaviour
{
    protected Player player;
    [SerializeField] protected float coolTime;
    protected float coolDownTimer;
    // UI
    [SerializeField] protected DashView view;

    private void Start()
    {
        coolDownTimer = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDownTimer <= coolTime)
        { 
            coolDownTimer += Time.deltaTime;
            view.UpdateCooltime(coolDownTimer, coolTime);
        }
    }

    public void UseSkillInputAction(InputAction.CallbackContext context)
    {
        if(coolDownTimer >= coolTime)
        {
            coolDownTimer = 0f;
            UseSkill();
        }
    }

    public abstract void UseSkill();

    public virtual void Init(Player player)
    {
        this.player = player;
    }
}
