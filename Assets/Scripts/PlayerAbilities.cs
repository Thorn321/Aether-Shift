using System.Collections;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Unlocks")]
    [SerializeField] private bool doubleJumpUnlocked = true;
    [SerializeField] private bool dashUnlocked = true;

    [Header("Double Jump")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float jumpCooldown = 0.12f;
    [SerializeField] private float groundResetLock = 0.05f;

    // síla double jumpu
    [SerializeField, Range(0.5f, 1f)] private float doubleJumpMultiplier = 0.85f;

    private int jumpsLeft;
    private float lastJumpTime = -999f;
    private float lockGroundResetUntil = -999f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    private float lastDashTime = -999f;
    private bool isDashing;

    [Header("Graphics (for facing direction)")]
    [SerializeField] private Transform graphics; // sem přetáhni Graphics child (tam kde flipuješ sprite)

    private PlayerMovement movement;
    private Rigidbody2D rb;

    private bool wasGrounded;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        if (graphics == null)
            graphics = transform.Find("Graphics");
    }

    private void Start()
    {
        ResetJumps();
        wasGrounded = movement != null && movement.IsGrounded;
    }

    private void Update()
    {
        HandleJumpInput();
        HandleDashInput();
        HandleLandingReset();
    }

    // ---------------- LANDING RESET ----------------

    private void HandleLandingReset()
    {
        if (movement == null)
            return;

        bool groundedNow = movement.IsGrounded;

        // Reset jen při dopadu (false -> true) + po locku
        if (!wasGrounded && groundedNow && Time.time >= lockGroundResetUntil)
            ResetJumps();

        wasGrounded = groundedNow;
    }

    // ---------------- JUMP ----------------

    private void ResetJumps()
    {
        jumpsLeft = doubleJumpUnlocked ? maxJumps : 1;
    }

    private void HandleJumpInput()
    {
        if (!Input.GetButtonDown("Jump"))
            return;

        if (isDashing)
            return;

        if (Time.time - lastJumpTime < jumpCooldown)
            return;

        if (movement == null)
            return;

        // Pokud double jump není odemčený -> skok jen ze země
        if (!doubleJumpUnlocked && !movement.IsGrounded)
            return;

        if (jumpsLeft <= 0)
            return;

        DoJump();
        jumpsLeft--;

        lastJumpTime = Time.time;
        lockGroundResetUntil = Time.time + groundResetLock; // zabrání okamžitému resetu skoků po odrazu
    }

    private void DoJump()
    {
        // když stojíš na zemi => plná síla
        // když nejsi na zemi (double jump) => slabší
        float force = movement.IsGrounded ? movement.JumpForce : movement.JumpForce * doubleJumpMultiplier;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        movement.PlayJumpAnim();
    }

    // ---------------- DASH ----------------

    private void HandleDashInput()
    {
        if (!dashUnlocked)
            return;

        if (!Input.GetKeyDown(dashKey))
            return;

        if (isDashing)
            return;

        if (Time.time - lastDashTime < dashCooldown)
            return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        lastDashTime = Time.time;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Směr: když držíš input, podle inputu. Když stojíš, podle facing (Graphics scale)
        float input = Input.GetAxisRaw("Horizontal");
        float dir;

        if (Mathf.Abs(input) > 0.01f)
        {
            dir = Mathf.Sign(input);
        }
        else
        {
            // fallback: podle toho, kam se hráč dívá (Graphics)
            dir = (graphics != null && graphics.localScale.x < 0f) ? -1f : 1f;
        }

        float t = 0f;
        while (t < dashDuration)
        {
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    // ---------------- PUBLIC API (unlock) ----------------

    public void UnlockDoubleJump()
    {
        doubleJumpUnlocked = true;
        ResetJumps();
    }

    public void UnlockDash()
    {
        dashUnlocked = true;
    }
}
