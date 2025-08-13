using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;   // Player/Move (Vector2)
    [SerializeField] private InputActionReference jumpAction;   // Player/Jump (Button)

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 8f;

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

    [Header("Visual")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator; // Parameters: "Speed"(Float), "Grounded"(Bool), "VelY"(Float)

    private Rigidbody2D rb;
    private Vector2 input;
    private float inputX;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        if (!sprite) sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
    }
    private void OnDisable()
    {
        moveAction?.action.Disable();
        jumpAction?.action.Disable();
    }

    private void Update()
    {
        // Read Input
        input = moveAction ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        if (input.x > 0)
            inputX = input.x / input.x;
        else if (input.x < 0)
            inputX = -input.x / input.x;
        else inputX = 0;
            Debug.Log(inputX);
        // Ground Check
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        bool groundedNow = Physics2D.OverlapCircle(checkPos, groundRadius, groundLayers);

        // Reset Jump Count
        if (!wasGrounded && groundedNow && Time.time >= blockResetUntil)
        {
            jumpCount = maxJumpCount;
        }
        isGrounded = groundedNow;

        // 점프 (단순)
        if (jumpAction != null && jumpAction.action.WasPressedThisFrame() && jumpCount > 0)
        { 
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce); 
            jumpCount--;

            // 점프 직후 '지면 리셋'을 잠깐 막아버리기
            blockResetUntil = Time.time + groundResetBlock;
            isGrounded = false;        // 같은 프레임에 다시 리셋되는 것 방지
            wasGrounded = false;       // 전이 상태 재정렬
        }

        // 좌우 반전
        if (sprite && Mathf.Abs(input.x) > 0.01f)
            sprite.flipX = input.x < 0f;

        // 애니메이터 파라미터
        if (animator)
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(inputX));   // 걷기 전이용
            animator.SetFloat("VelY", rb.linearVelocityY);    // 점프/낙하 전이용
        }
    }

    private void FixedUpdate()
    {
        // 가속/감속 없이 즉시 반영
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocityY); 

    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(checkPos, groundRadius);
    }
#endif
}
