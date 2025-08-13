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
    [SerializeField] private float groundResetBlock = 0.05f; // ���� ���� ���� ���� �ð�
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

        // ���� (�ܼ�)
        if (jumpAction != null && jumpAction.action.WasPressedThisFrame() && jumpCount > 0)
        { 
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce); 
            jumpCount--;

            // ���� ���� '���� ����'�� ��� ���ƹ�����
            blockResetUntil = Time.time + groundResetBlock;
            isGrounded = false;        // ���� �����ӿ� �ٽ� ���µǴ� �� ����
            wasGrounded = false;       // ���� ���� ������
        }

        // �¿� ����
        if (sprite && Mathf.Abs(input.x) > 0.01f)
            sprite.flipX = input.x < 0f;

        // �ִϸ����� �Ķ����
        if (animator)
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(inputX));   // �ȱ� ���̿�
            animator.SetFloat("VelY", rb.linearVelocityY);    // ����/���� ���̿�
        }
    }

    private void FixedUpdate()
    {
        // ����/���� ���� ��� �ݿ�
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
