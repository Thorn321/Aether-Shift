using System.Collections;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Unlocks")]
    [SerializeField] private bool doubleJumpUnlocked = true;
    [SerializeField] private bool dashUnlocked = true;

    [Header("Double Jump")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float doubleJumpMultiplier = 0.85f;

    private int jumpsLeft;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;

    private float lastDashTime;
    private bool isDashing;
    private bool airDashUsed;

    [Header("Graphics")]
    [SerializeField] private Transform graphics;
    private SpriteRenderer graphicsSR;

    [Header("Afterimage")]
    [SerializeField] private GameObject afterimagePrefab;
    [SerializeField] private float afterimageSpawnRate = 0.03f;

    [Header("Effects")]
    [SerializeField] private TrailRenderer trail;

    [Header("Audio")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip doubleJumpSound;

    private PlayerMovement movement;
    private Rigidbody2D rb;
    private Animator anim;
    private static readonly int DashHash = Animator.StringToHash("Dash");
    private bool wasGrounded;

    private static readonly Vector3 normalScale = new(1f, 1f, 1f);

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        if (!graphics)
            graphics = transform.Find("Graphics");

        if (graphics)
            graphicsSR = graphics.GetComponent<SpriteRenderer>();

        if (trail)
            trail.emitting = false;
    }

    private void Start()
    {
        ResetJumps();
        wasGrounded = movement.IsGrounded;
    }

    private void Update()
    {
        HandleJump();
        HandleDash();
        HandleLandingReset();
    }

    // ===================== RESET =====================

    private void HandleLandingReset()
    {
        if (movement.IsGrounded && !wasGrounded)
        {
            ResetJumps();
        }

        wasGrounded = movement.IsGrounded;
    }

    private void ResetJumps()
    {
        jumpsLeft = doubleJumpUnlocked ? maxJumps : 1;
        airDashUsed = false;
    }

    // ===================== JUMP =====================

    private void HandleJump()
    {
        if (!Input.GetButtonDown("Jump")) return;
        if (isDashing) return;

        if (!doubleJumpUnlocked && !movement.IsGrounded)
            return;

        if (jumpsLeft <= 0)
            return;

        float force = movement.IsGrounded
            ? movement.JumpForce
            : movement.JumpForce * doubleJumpMultiplier;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);

        movement.PlayJumpAnim();

        if (doubleJumpSound)
            SFXManager.Instance.PlaySound(doubleJumpSound, 1.6f);

        jumpsLeft--;
    }

    // ===================== DASH =====================

    private void HandleDash()
    {
        if (!dashUnlocked) return;
        if (!Input.GetKeyDown(dashKey)) return;
        if (isDashing) return;
        if (Time.time - lastDashTime < dashCooldown) return;

        if (!movement.IsGrounded && airDashUsed)
            return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        PlayDashAnim();

        if (!movement.IsGrounded)
            airDashUsed = true;

        float input = Input.GetAxisRaw("Horizontal");
        float dir = Mathf.Abs(input) > 0.01f
            ? Mathf.Sign(input)
            : (graphics.localScale.x < 0 ? -1 : 1);

        if (dashSound)
            SFXManager.Instance.PlaySoundRandomPitch(dashSound, 1f, 1.3f);

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        if (trail) trail.emitting = true;

        //StartCoroutine(Squash());
        StartCoroutine(SpawnAfterimages());

        float t = 0f;

        while (t < dashDuration)
        {
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0);
            t += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;

        if (trail) trail.emitting = false;

        isDashing = false;
    }

    // ===================== AFTERIMAGES =====================

    private IEnumerator SpawnAfterimages()
    {
        while (isDashing)
        {
            if (afterimagePrefab && graphicsSR)
            {
                GameObject img = Instantiate(afterimagePrefab, graphics.position, graphics.rotation);

                SpriteRenderer sr = img.GetComponent<SpriteRenderer>();

                sr.sprite = graphicsSR.sprite;
                sr.flipX = graphicsSR.flipX;

                Color c = sr.color;
                c.a = 0.5f;
                sr.color = c;

                img.transform.localScale = graphics.localScale;
            }

            yield return new WaitForSeconds(afterimageSpawnRate);
        }
    }

    // ===================== JUICE =====================

    private IEnumerator Squash()
    {
        if (!graphics) yield break;

        Vector3 original = graphics.localScale;
        Vector3 stretched = new Vector3(1.8f, 0.55f, 1f);

        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            graphics.localScale = Vector3.Lerp(original, stretched, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        graphics.localScale = stretched;

        yield return new WaitForSeconds(0.05f);

        t = 0f;

        while (t < duration)
        {
            graphics.localScale = Vector3.Lerp(stretched, original, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        graphics.localScale = original;
    }

    // ===================== ANIMATION ====================

    public void PlayDashAnim()
    {
        if (!anim) return;

        anim.ResetTrigger(DashHash);
        anim.SetTrigger(DashHash);
    }

    // ===================== UNLOCKS =====================

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