using Unity.VisualScripting;
using UnityEngine;

public enum SurfaceType
{
    Grass,
    Stone
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckOffset = new(0, -0.6f);
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Wall Check")]
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private Vector2 wallBottomOffset = new(0, -0.4f);
    [SerializeField] private Vector2 wallTopOffset = new(0, 0.4f);

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] grassFootsteps;
    [SerializeField] private AudioClip[] stoneFootsteps;
    [SerializeField] private float footstepInterval = 0.4f;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    [Header("Misc")]
    [SerializeField] private Transform graphics;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fallThreshold = -10f;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private Animator anim;



    private bool isGrounded;
    private bool wasGrounded;
    private bool wallLeft;
    private bool wallRight;

    private float footstepTimer;

    private SurfaceType currentSurface = SurfaceType.Stone;
    private Vector2 checkpoint;

    // ---------------- ANIMATION CACHE ----------------
    private float lastSpeed;

    // ---------------- HASHES ----------------
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int YVelHash = Animator.StringToHash("YVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DashHash = Animator.StringToHash("Dash");

    public float SpeedMultiplier { get; set; } = 1f;
    public bool IsGrounded => isGrounded;
    public float JumpForce => jumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponentInChildren<Animator>();

        if (!graphics)
            graphics = transform.Find("Graphics");
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
            return;

        CheckGround();
        CheckWalls();
        UpdateSurface();

        HandleMovement();
        UpdateAnimator();

        CheckFall();
    }

    // ---------------- GROUND ----------------

    private void CheckGround()
    {
        Vector2 pos = (Vector2)transform.position + groundCheckOffset;

        bool groundedNow = Physics2D.OverlapCircle(pos, groundCheckRadius, groundLayer);

        if (!wasGrounded && groundedNow && landSound != null)
            SFXManager.Instance.PlaySoundRandomPitch(landSound, 0.9f, 1.3f);

        wasGrounded = groundedNow;
        isGrounded = groundedNow;
    }

    // ---------------- WALLS ----------------

    private void CheckWalls()
    {
        Vector2 pos = transform.position;

        Vector2 bottom = pos + wallBottomOffset;
        Vector2 top = pos + wallTopOffset;

        float dist = wallCheckDistance + 0.5f;

        wallRight =
            Physics2D.Raycast(bottom, Vector2.right, dist, groundLayer) ||
            Physics2D.Raycast(top, Vector2.right, dist, groundLayer);

        wallLeft =
            Physics2D.Raycast(bottom, Vector2.left, dist, groundLayer) ||
            Physics2D.Raycast(top, Vector2.left, dist, groundLayer);
    }

    // ---------------- MOVEMENT ----------------

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if ((input > 0 && wallRight) || (input < 0 && wallLeft))
            input = 0;

        rb.linearVelocity = new Vector2(input * moveSpeed * SpeedMultiplier, rb.linearVelocity.y);

        if (graphics && input != 0)
            graphics.localScale = new Vector3(Mathf.Sign(input), 1, 1);

        HandleFootsteps();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (jumpSound)
            SFXManager.Instance.PlaySoundRandomPitch(jumpSound, 0.9f, 1.3f);

        PlayJumpAnim();
    }

    private void UpdateSurface()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            (Vector2)transform.position + groundCheckOffset,
            Vector2.down,
            1f,
            groundLayer
        );

        if (hit.collider != null && hit.collider.TryGetComponent(out Surface surface))
            currentSurface = surface.type;
        else
            currentSurface = SurfaceType.Stone;
    }

    // ---------------- FOOTSTEPS ----------------

    private void HandleFootsteps()
    {
        if (!isGrounded || Mathf.Abs(rb.linearVelocity.x) < 0.1f)
        {
            footstepTimer = 0;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer > 0) return;

        AudioClip[] set = currentSurface == SurfaceType.Grass
            ? grassFootsteps
            : stoneFootsteps;

        if (set.Length > 0)
        {
            var clip = set[Random.Range(0, set.Length)];
            SFXManager.Instance.PlaySoundRandomPitch(clip, 0.9f, 1.4f);
        }

        footstepTimer = footstepInterval;
    }

    // ---------------- ANIMATOR (CLEAN BLEND TREE SYSTEM) ----------------

    private void UpdateAnimator()
    {
        if (!anim) return;

        float speed = Mathf.Abs(rb.linearVelocity.x);

        if (speed < 0.05f)
            speed = 0f;

        float normalizedSpeed = speed / moveSpeed;

        // debug (SPRÁVNĚ)
        Debug.Log("Speed: " + normalizedSpeed);
        Debug.Log("Grounded: " + isGrounded);

        anim.SetFloat(SpeedHash, normalizedSpeed);
        anim.SetBool(GroundedHash, isGrounded);
        anim.SetFloat(YVelHash, rb.linearVelocity.y);
    }

    public void PlayJumpAnim()
    {
        anim?.SetTrigger(JumpHash);
    }

    public void PlayHurtAnim()
    {
        anim?.SetTrigger(HurtHash);
    }

    public void PlayDashAnim()
    {
        anim?.SetTrigger(DashHash);
    }

    // ---------------- FALL ----------------

    private void CheckFall()
    {
        if (transform.position.y > fallThreshold) return;

        if (health)
            health.TakeDamage(1);
        else
            ReturnToCheckpoint();

        PlayHurtAnim();
    }

    // ---------------- CHECKPOINT ----------------

    public void SetCheckpoint(Vector2 pos) => checkpoint = pos;

    public void ReturnToCheckpoint()
    {
        rb.linearVelocity = Vector2.zero;

        transform.position = checkpoint != Vector2.zero
            ? checkpoint
            : (spawnPoint ? (Vector2)spawnPoint.position : transform.position);
    }
}