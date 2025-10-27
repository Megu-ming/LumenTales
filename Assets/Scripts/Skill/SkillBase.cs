using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField] InputActionAsset inputAsset;
    [SerializeField] float coolTime;
    private InputAction skillAction;
    private float coolDownTimer;

    private void Start()
    {
        Init();
    }

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
    protected virtual void Init() {}
}
