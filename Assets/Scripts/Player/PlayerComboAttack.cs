using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IDamageable { void TakeDamage(int ammount); }

[System.Serializable]
public struct AttackStep
{
    public string animTrigger;
    public float windup;
    public float active;
    public float recovery;
    public Vector2 hitOffset;
    public float hitRadius;
    public int damage;
}

public class PlayerComboAttack : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference attackAction;

    [Header("Combo Setting")]
    [SerializeField] private AttackStep[] steps = new AttackStep[3];
    [SerializeField] private float comboTimeout = 0.5f;
    [SerializeField] private float inputBuffer = 0.2f;
    [SerializeField] private LayerMask hitLayers;

    private SpriteRenderer sprite;
    private Animator animator;

    [SerializeField] private int currentStepIndex = -1;  // -1 : No Combo, 0~2 : currentStep
    private bool inAttack;              
    private bool queuedNext;
    private float lastAttackEndTime;

    private void Awake()
    {   
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if(steps == null||steps.Length == 0)
        {
            steps = new AttackStep[3];
            steps[0] = new AttackStep { animTrigger = "Attack_01", windup = 0.05f, active = 0.08f, recovery = 0.12f, hitOffset = new Vector2(0.6f, 0f), hitRadius = 0.35f, damage = 1 };
            steps[1] = new AttackStep { animTrigger = "Attack_02", windup = 0.06f, active = 0.10f, recovery = 0.14f, hitOffset = new Vector2(0.7f, 0f), hitRadius = 0.37f, damage = 1 };
            steps[2] = new AttackStep { animTrigger = "Attack_03", windup = 0.08f, active = 0.12f, recovery = 0.18f, hitOffset = new Vector2(0.85f, 0.05f), hitRadius = 0.40f, damage = 2 };
        }
    }

    private void OnEnable() { attackAction?.action.Enable(); }
    private void OnDisable() { attackAction?.action.Disable(); }

    private void Update() 
    {
        if(attackAction != null && attackAction.action.WasPressedThisFrame())
        {
            if (inAttack)
                queuedNext = true;
            else
            {
                bool timedOut = (Time.time - lastAttackEndTime) > comboTimeout;
                if (currentStepIndex < 0 || timedOut)
                    currentStepIndex = 0;
                else
                    currentStepIndex = Mathf.Clamp(currentStepIndex + 1, 0, steps.Length - 1);

                StartCoroutine(RunStep(currentStepIndex));
            }
        }
    }

    private IEnumerator RunStep(int index)
    {
        inAttack = true;
        queuedNext = false;

        var step = steps[index];

        if(animator && !string.IsNullOrEmpty(step.animTrigger))
            animator.SetTrigger(step.animTrigger);

        if(step.windup>0f) yield return new WaitForSeconds(step.windup);

        DoHit(step);

        if (step.active > 0f) yield return new WaitForSeconds(step.active);

        if (step.recovery > 0f) yield return new WaitForSeconds(step.recovery);

        inAttack = false;
        lastAttackEndTime = Time.time;

        if (queuedNext && index + 1 < steps.Length)
        {
            currentStepIndex = index + 1;
            StartCoroutine(RunStep(currentStepIndex));
        }
        else StartCoroutine(ComboTimeoutGuard());
    }

    private IEnumerator ComboTimeoutGuard()
    {
        float start = Time.time;
        while(Time.time-start<comboTimeout)
        {
            if(attackAction!=null&&attackAction.action.WasPressedThisFrame())
            {
                int next = Mathf.Clamp(currentStepIndex + 1, 0, steps.Length - 1);
                if (next != currentStepIndex)
                {
                    currentStepIndex = next;
                    StartCoroutine(RunStep(currentStepIndex));
                    yield break;
                }
            }
            yield return null;
        }
        currentStepIndex = -1;
        queuedNext = false;
    }

    private void DoHit(AttackStep step)
    {
        Vector2 off = step.hitOffset;
        if(sprite && sprite.flipX) off.x = -Mathf.Abs(off.x); else off.x = Mathf.Abs(off.x);
        Vector2 center = (Vector2)transform.position + off;

        var hits = Physics2D.OverlapCircleAll(center,step.hitRadius,hitLayers);
        for(int i=0;i<hits.Length;i++)
        {
            // Damage Logic
            // TODO
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if(steps == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.35f);
        foreach(var step in steps)
        {
            Vector2 off = step.hitOffset;
            if (sprite && sprite.flipX) off.x = -Mathf.Abs(off.x); else off.x = Mathf.Abs(off.x);
            Vector2 center = (Vector2)transform.position + off;
            Gizmos.DrawSphere(center, step.hitRadius);
        }
    }

#endif
}
