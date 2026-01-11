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

    [Header("Starting Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Fall Settings")]
    [SerializeField] private float fallThreshold = -10f;

    private Rigidbody2D rb;
    private bool wallLeft;
    private bool wallRight;
    private bool isGrounded;
    private Vector2 latestCheckpoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckFall();
        CheckGround();
        CheckWalls();
        HandleMovement();
        HandleJump();
    }

    // ---------------- CHECKS ----------------

    private void CheckFall()
    {
        if (transform.position.y < fallThreshold)
            ReturnToCheckpoint();
    }

    private void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;

        isGrounded =
            Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        Debug.DrawRay(
            transform.position + (Vector3)groundCheckOffset,
            Vector3.down * groundCheckRadius,
            isGrounded ? Color.green : Color.red
        );
    }

    private void CheckWalls()
    {
        Vector2 pos = transform.position;
        Vector2 bottomOrigin = pos + wallCheckBottomOffset;
        Vector2 topOrigin = pos + wallCheckTopOffset;
        float rayLength = wallCheckDistance + 0.5f;

        wallRight =
            Physics2D.Raycast(bottomOrigin, Vector2.right, rayLength, groundLayer) ||
            Physics2D.Raycast(topOrigin, Vector2.right, rayLength, groundLayer);

        wallLeft =
            Physics2D.Raycast(bottomOrigin, Vector2.left, rayLength, groundLayer) ||
            Physics2D.Raycast(topOrigin, Vector2.left, rayLength, groundLayer);

        Debug.DrawRay(bottomOrigin, Vector2.right * rayLength, Color.red);
        Debug.DrawRay(bottomOrigin, Vector2.left * rayLength, Color.red);
        Debug.DrawRay(topOrigin, Vector2.right * rayLength, Color.red);
        Debug.DrawRay(topOrigin, Vector2.left * rayLength, Color.red);
    }

    // ---------------- MOVEMENT ----------------

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if ((moveInput > 0 && wallRight) || (moveInput < 0 && wallLeft))
            moveInput = 0;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // ---------------- CHECKPOINTS ----------------

    public void SetCheckpoint(Vector2 pos)
    {
        latestCheckpoint = pos;
    }

    public void ReturnToCheckpoint()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = latestCheckpoint != Vector2.zero
            ? latestCheckpoint
            : (Vector2)spawnPoint.position;
    }
}