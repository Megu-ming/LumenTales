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
        skillAction = inputAsset.FindActionMap("Player").FindAction("Skill");
        skillAction.performed += UseSkillInputAction;
        Init();
    }


    // Update is called once per frame
    void Update()
    {
        if (coolDownTimer > 0f)
            coolDownTimer -= Time.deltaTime;
    }

    private void OnDestroy()
    {
        skillAction.performed -= UseSkillInputAction;
    }

    private void UseSkillInputAction(InputAction.CallbackContext context)
    {
        if(coolDownTimer <= 0f)
        {
            coolDownTimer += coolTime;
            UseSkill();
        }
    }

    protected abstract void UseSkill();
    protected virtual void Init()
    {

    }
}
