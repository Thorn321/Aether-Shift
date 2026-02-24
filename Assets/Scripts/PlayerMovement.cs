using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Environment Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer; // zatím nevyužito
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private Vector2 wallCheckBottomOffset = new Vector2(0, -0.4f);
    [SerializeField] private Vector2 wallCheckTopOffset = new Vector2(0, 0.4f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.6f);
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Starting Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Fall Settings")]
    [SerializeField] private float fallThreshold = -10f;

    [Header("Graphics")]
    [SerializeField] private Transform graphics;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private Animator anim;

    private bool wallLeft;
    private bool wallRight;
    private bool isGrounded;

    private Vector2 latestCheckpoint;

    // ===== SPEED BOOST SUPPORT =====
    // 1 = normální rychlost, 1.5 = +50% rychlost atd.
    public float SpeedMultiplier { get; set; } = 1f;

    // ---- Public getters pro ability systém ----
    public bool IsGrounded => isGrounded;
    public Rigidbody2D RB => rb;
    public float JumpForce => jumpForce;

    // Pokud někde používáš MoveSpeed, je lepší vracet aktuální rychlost (už s multiplikátorem)
    public float MoveSpeed => moveSpeed * SpeedMultiplier;

    // (Volitelně) když chceš někde základní rychlost bez buffu
    public float BaseMoveSpeed => moveSpeed;

    // Hash parametry Animatoru
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponentInChildren<Animator>();

        if (graphics == null)
            graphics = transform.Find("Graphics");
    }

    private void Update()
    {
        CheckFall();
        CheckGround();
        CheckWalls();
        HandleMovement();
        UpdateAnimatorParams();
    }

    // ---------------- CHECKS ----------------

    private void CheckFall()
    {
        if (transform.position.y >= fallThreshold)
            return;

        if (health != null)
        {
            health.TakeDamage(1);

            if (anim != null)
                anim.SetTrigger(HurtHash);
        }
        else
        {
            ReturnToCheckpoint();
        }
    }

    private void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;

        isGrounded =
            Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);
        // || Physics2D.OverlapCircle(checkPos, groundCheckRadius, enemyLayer);

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
        float moveInput = Input.GetAxisRaw("Horizontal");

        // blokace směru při zdi
        if ((moveInput > 0 && wallRight) || (moveInput < 0 && wallLeft))
            moveInput = 0f;

        // použij moveSpeed * SpeedMultiplier
        float currentSpeed = moveSpeed * SpeedMultiplier;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Flip grafiky
        if (graphics != null && moveInput != 0f)
        {
            float scaleX = moveInput > 0f ? 1f : -1f;
            graphics.localScale = new Vector3(scaleX, 1f, 1f);
        }
    }

    // ---------------- ANIMATION ----------------

    private void UpdateAnimatorParams()
    {
        if (anim == null)
            return;

        // Reálná rychlost
        float speed = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat(SpeedHash, speed);

        anim.SetBool(GroundedHash, isGrounded);
    }

    public void PlayJumpAnim()
    {
        if (anim != null)
            anim.SetTrigger(JumpHash);
    }

    // ---------------- CHECKPOINTS ----------------

    public void SetCheckpoint(Vector2 pos)
    {
        latestCheckpoint = pos;
    }

    public void ReturnToCheckpoint()
    {
        rb.linearVelocity = Vector2.zero;

        transform.position =
            latestCheckpoint != Vector2.zero
                ? latestCheckpoint
                : (spawnPoint != null
                    ? (Vector2)spawnPoint.position
                    : (Vector2)transform.position);
    }

    public void BackToSpawn()
    {
        rb.linearVelocity = Vector2.zero;

        transform.position =
            spawnPoint != null
                ? (Vector2)spawnPoint.position
                : Vector2.zero;
    }
}