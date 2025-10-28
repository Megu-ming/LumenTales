using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : CreatureController
{
    Player player;
    [Header("Skills")]
    [SerializeField] SkillBase skill;

    #region MovementVariables
    [Header("Movement")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float sprintMul = 1.2f;
    [SerializeField] private bool isMoving = false;
    public float moveSpeed;
    public float CurrentSpeed
    {
        get
        {
            if (isDash) return moveSpeed;

            if (!CanMove) return 0f;
            if (!IsMoving) return 0f;

            return IsSprint ? moveSpeed * sprintMul : moveSpeed;
        }
    }
    private bool isDash;
    public bool IsDash
    {
        get { return isDash; }
        set { isDash = value; animator.SetBool(AnimationStrings.isDash, value); }
    }

    public bool IsMoving { 
        get 
        { return isMoving; } 
        set
        {
            isMoving = value;
            animator.SetBool(AnimationStrings.move, value);
        }
    }

    [SerializeField] private bool _isSprint = false;
    public bool IsSprint
    {
        get { return _isSprint; }
        set
        {
            _isSprint = value;
            animator.SetBool(AnimationStrings.sprint, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight { get { return _isFacingRight; } private set { 
        if(_isFacingRight!=value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
        _isFacingRight = value;
        } }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public bool IsAlive 
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }
    #endregion

    [Header("Ground Check")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.25f);
    [SerializeField] private float groundRadius = 0.05f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Jump Setting")]
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int jumpCount;
    private bool wasGrounded;
    [SerializeField] private float groundResetBlock = 0.05f; // 점프 직후 리셋 금지 시간
    private float blockResetUntil = -1f;

    private Vector2 moveInput;
    private bool isGrounded;

    public bool IsGrounded 
    { 
        get { return isGrounded; }
        set { animator.SetBool(AnimationStrings.isGrounded, value); }
    }

    public bool IsAttack
    {
        get { return animator.GetBool(AnimationStrings.isAttack); }
    }

    public event Action OnInteractionEvent;
    public bool isConversation = false;

    // Dash CoolTime
    public float dashCooltime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;
    }

    public void Init(Player player)
    {
        this.player = player;
        skill.Init(player);
    }

    private void Update()
    {
        // Ground Check
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        bool groundedNow = Physics2D.OverlapCircle(checkPos, groundRadius, groundLayers);

        // Reset Jump Count
        if (!wasGrounded && groundedNow && Time.time >= blockResetUntil)
        {
            jumpCount = maxJumpCount;
        }
        IsGrounded = groundedNow;

        // 애니메이터 파라미터
        if (animator)
        {
            animator.SetFloat(AnimationStrings.velY, rb.linearVelocityY);    // 점프/낙하 전이용
        }
    }

    private void FixedUpdate()
    {
        // 이동 잠금: 대시가 아닐 때만 잠금 적용
        if (!CanMove && !isDash)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
            return;
        }

        float xVel;

        if (isDash)
        {
            // 대시 중: 입력 무시, 바라보는 방향 × CurrentSpeed
            xVel = (IsFacingRight ? 1f : -1f) * CurrentSpeed;
            rb.linearVelocity = new Vector2(xVel, 0);
        }
        else
        {
            // 일반 이동: 입력 × CurrentSpeed (스프린트/정지/잠금은 CurrentSpeed 내부에서 처리)
            xVel = moveInput.x * CurrentSpeed;
            rb.linearVelocity = new Vector2(xVel, rb.linearVelocityY);
        }
    }

    public override void OnDead()
    {
        
    }

    #region InputFunction
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isConversation) return;

        if (context.canceled)
        {
            moveInput = Vector2.zero;
            IsMoving = false;
            return;
        }

        if (isDash) return;

        moveInput = context.ReadValue<Vector2>();
        if(IsAlive && IsAttack is false)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else IsMoving = false;

        if (moveInput.x > 0) moveInput.x = 1f;
        else if (moveInput.x < 0) moveInput.x = -1f;
        else moveInput.x = 0f;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isConversation || isDash) return;
        if (context.performed && jumpCount > 0 && CanMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpCount--;
            // 점프 직후 '지면 리셋'을 잠깐 막아버리기
            blockResetUntil = Time.time + groundResetBlock;
            IsGrounded = false;        // 같은 프레임에 다시 리셋되는 것 방지
            wasGrounded = false;       // 전이 상태 재정렬
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed)
            IsSprint = true;
        else if (context.canceled)
            IsSprint = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isConversation || isDash) return;
        animator.SetTrigger(AnimationStrings.attack);
    }

    public void OnEquipmentToggle()
    {
        if (isConversation) return;
        player.InventoryController?.RequestToggleEquipUI();
    }

    public void OnInvenUIToggle()
    {
        if (isConversation) return;
        player.InventoryController?.RequestToggleInvenUI();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            CallInteractEvent();
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocityY + knockback.y);
    }

    #endregion

    #region Functions
    // Setting Facing Direction Function
    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }

    }

    public void SetGravity(float val)
    {
        rb.gravityScale = val;
    }

    public void CallInteractEvent() => OnInteractionEvent?.Invoke();
    private void SetConversationsState(bool state)
    {
        isConversation = state;
        if(state)
        {
            moveInput = Vector2.zero;
            IsMoving = false;
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPos, groundRadius);
    }
#endif
}