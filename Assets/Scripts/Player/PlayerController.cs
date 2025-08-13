using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    #region MovementInput
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;   // Player/Move (Vector2)
    [SerializeField] private InputActionReference jumpAction;   // Player/Jump (Button)
    [SerializeField] private InputActionReference sprintAction;   // Player/Sprint (Button)

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
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
    [SerializeField] private float groundResetBlock = 0.05f; // ���� ���� ���� ���� �ð�
    private float blockResetUntil = -1f;
    #endregion

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;
    private float inputX;
    private bool isGrounded;
    private float isSprint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
        sprintAction?.action.Enable();
    }
    private void OnDisable()
    {
        moveAction?.action.Disable();
        jumpAction?.action.Disable();
        sprintAction?.action.Disable();
    }

    private void Update()
    {
        // Read Input
        inputX = moveAction ? moveAction.action.ReadValue<Vector2>().x : 0.0f;
        if (inputX > 0)
            inputX = inputX / inputX;
        else if (inputX < 0)
            inputX = -inputX / inputX;
        else inputX = 0;

        isSprint = sprintAction.action.ReadValue<float>();
       
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
        if (sprite && Mathf.Abs(inputX) > 0.01f)
            sprite.flipX = inputX < 0f;

        // �ִϸ����� �Ķ����
        if (animator)
        {
            animator.SetBool("Grounded", isGrounded);
            animator.SetFloat("Speed", Mathf.Abs(inputX));   // �ȱ� ���̿�
            animator.SetFloat("VelY", rb.linearVelocityY);    // ����/���� ���̿�
            animator.SetFloat("Sprint", isSprint);
        }

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = isSprint > 0 ? new Vector2(inputX * moveSpeed * sprintMul, rb.linearVelocityY) : new Vector2(inputX * moveSpeed, rb.linearVelocityY);
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
