using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    #region MovementVariables
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float sprintMul = 1.5f;

    [Header("Ground Check")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    [SerializeField] private float groundRadius = 0.05f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Jump Setting")]
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int jumpCount;
    private bool wasGrounded;
    [SerializeField] private float groundResetBlock = 0.05f; // 점프 직후 리셋 금지 시간
    private float blockResetUntil = -1f;
    #endregion

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private Vector2 moveInput;
    private bool isGrounded;
    private float isSprint;
    public bool isAttacking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
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
        isGrounded = groundedNow;

        // 좌우 반전
        if (sprite && Mathf.Abs(moveInput.x) > 0.01f)
            sprite.flipX = moveInput.x < 0f;

        // 애니메이터 파라미터
        if (animator)
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));   // 걷기 전이용
            animator.SetFloat("VelY", rb.linearVelocityY);    // 점프/낙하 전이용
            animator.SetFloat("Sprint", isSprint);
        }

    }

    private void FixedUpdate()
    {
        //if (!isAttacking)
        //    rb.linearVelocity = isSprint > 0 ? new Vector2(moveInput.x * moveSpeed * sprintMul, rb.linearVelocityY) : new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);
        //else
        //    rb.linearVelocity = Vector2.zero;

        if (!isAttacking)
        {
            float sx = (isSprint > 0 ? sprintMul : 1f);
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed * sx, rb.linearVelocityY); // ★ deltaTime 제거 + velocity 통일
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPos, groundRadius);
    }
#endif

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput.x > 0)
            moveInput.x = 1f;
        else if (moveInput.x < 0)
            moveInput.x = -1f;
        else
            moveInput.x = 0f;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            jumpCount--;
            // 점프 직후 '지면 리셋'을 잠깐 막아버리기
            blockResetUntil = Time.time + groundResetBlock;
            isGrounded = false;        // 같은 프레임에 다시 리셋되는 것 방지
            wasGrounded = false;       // 전이 상태 재정렬
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
            isSprint = context.ReadValue<float>();
        else if (context.canceled)
            isSprint = 0.0f;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 공격 로직 구현
            Debug.Log("Attack performed");
            animator.SetTrigger("Attack_01");
        }
    }
}
