using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Environment Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private Vector2 wallCheckBottomOffset = new Vector2(0, -0.4f);
    [SerializeField] private Vector2 wallCheckTopOffset = new Vector2(0, 0.4f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.6f);
    [SerializeField] private float groundCheckRadius = 0.15f;

    private Rigidbody2D rb;
    private bool wallLeft;
    private bool wallRight;
    private bool isGrounded;
    private bool isKnocked = false;
    private float knockDuration = 0.2f;
    private float knockTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isKnocked)
        {
            knockTimer -= Time.deltaTime;
            if (knockTimer <= 0)
                isKnocked = false;
            return; // bìhem knockbacku neovládáš hráèe
        }

        CheckGround();
        CheckWalls();
        Move();
        Jump();
    }

    void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;

        // Detekuje zem nebo enemy
        isGrounded = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer)
            || Physics2D.OverlapCircle(checkPos, groundCheckRadius, enemyLayer);

        Debug.DrawRay(transform.position + (Vector3)groundCheckOffset, Vector3.down * groundCheckRadius, isGrounded ? Color.green : Color.red);
    }

    void CheckWalls()
    {
        Vector2 bottomOrigin = (Vector2)transform.position + wallCheckBottomOffset;
        Vector2 topOrigin = (Vector2)transform.position + wallCheckTopOffset;

        wallRight = Physics2D.Raycast(bottomOrigin, Vector2.right, wallCheckDistance + 0.5f, groundLayer)
                 || Physics2D.Raycast(topOrigin, Vector2.right, wallCheckDistance + 0.5f, groundLayer);

        wallLeft = Physics2D.Raycast(bottomOrigin, Vector2.left, wallCheckDistance + 0.5f, groundLayer)
                || Physics2D.Raycast(topOrigin, Vector2.left, wallCheckDistance + 0.5f, groundLayer);

        // Debug rays
        Debug.DrawRay(bottomOrigin, Vector2.right * (wallCheckDistance + 0.5f), Color.red);
        Debug.DrawRay(bottomOrigin, Vector2.left * (wallCheckDistance + 0.5f), Color.red);
        Debug.DrawRay(topOrigin, Vector2.right * (wallCheckDistance + 0.5f), Color.red);
        Debug.DrawRay(topOrigin, Vector2.left * (wallCheckDistance + 0.5f), Color.red);
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Block movement into the wall
        if ((moveInput > 0 && wallRight) || (moveInput < 0 && wallLeft))
            moveInput = 0;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction.x, direction.y + 0.5f) * force, ForceMode2D.Impulse);
        isKnocked = true;
        knockTimer = knockDuration;
    }
}