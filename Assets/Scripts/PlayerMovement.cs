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

    [Header("Graphics")]
    [SerializeField] private Transform graphics;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepInterval = 0.4f;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private Animator anim;

    private bool wallLeft;
    private bool wallRight;
    private bool isGrounded;
    private bool wasGrounded;
    private bool wasPausedLastFrame = false;

    private float footstepTimer;

    private Vector2 latestCheckpoint;

    public float SpeedMultiplier { get; set; } = 1f;

    public bool IsGrounded => isGrounded;
    public Rigidbody2D RB => rb;
    public float JumpForce => jumpForce;

    public float MoveSpeed => moveSpeed * SpeedMultiplier;
    public float BaseMoveSpeed => moveSpeed;

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
        bool isPaused = GameManager.Instance != null && GameManager.Instance.IsPaused;

        if (isPaused && !wasPausedLastFrame)
        {
            FreezePlayer();
        }
        else if (!isPaused && wasPausedLastFrame)
        {
            UnfreezePlayer();
        }

        wasPausedLastFrame = isPaused;

        if (isPaused) return;

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

        bool groundedNow =
            Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        if (!wasGrounded && groundedNow)
        {
            if (landSound != null)
                SFXManager.Instance.PlaySoundRandomPitch(landSound, 0.9f, 1.3f);
        }

        wasGrounded = groundedNow;
        isGrounded = groundedNow;
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
    }

    // ---------------- MOVEMENT ----------------

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if ((moveInput > 0 && wallRight) || (moveInput < 0 && wallLeft))
            moveInput = 0f;

        float currentSpeed = moveSpeed * SpeedMultiplier;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Flip
        if (graphics != null && moveInput != 0f)
        {
            float scaleX = moveInput > 0f ? 1f : -1f;
            graphics.localScale = new Vector3(scaleX, 1f, 1f);
        }

        // Footsteps
        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                if (footstepSounds.Length > 0)
                {
                    int i = Random.Range(0, footstepSounds.Length);
                    SFXManager.Instance.PlaySoundRandomPitch(footstepSounds[i], 0.9f, 1.5f);
                }

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (jumpSound != null)
                SFXManager.Instance.PlaySoundRandomPitch(jumpSound, 0.9f, 1.3f);

            PlayJumpAnim();
        }
    }

    // ---------------- ANIMATION ----------------

    private void UpdateAnimatorParams()
    {
        if (anim == null)
            return;

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

    // ---------------- FREEZE ----------------
    private void FreezePlayer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void UnfreezePlayer()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}