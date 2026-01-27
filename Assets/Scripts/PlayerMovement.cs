using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Environment Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer; // zatÌm nevyuûito, ale nech·v·m
    [SerializeField] private float wallCheckDistance = 0.05f;
    [SerializeField] private Vector2 wallCheckBottomOffset = new Vector2(0, -0.4f);
    [SerializeField] private Vector2 wallCheckTopOffset = new Vector2(0, 0.4f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.6f);
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Starting Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Fall Settings")]
    [SerializeField] private float fallThreshold = -10f;

    [Header("Animation")]
    [SerializeField] private float runThreshold01 = 0.8f; // jen pro debug / ladÏnÌ (Animator si ¯eöÌ transitions)

    [Header("Graphics")]
    [SerializeField] private Transform graphics;

    private Rigidbody2D rb;
    private PlayerHealth health;
    private Animator anim;

    private bool wallLeft;
    private bool wallRight;
    private bool isGrounded;
    private Vector2 latestCheckpoint;

    // Optimalizace: hash n·zv˘ parametr˘ (rychlejöÌ + mÈnÏ p¯eklep˘)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckFall();
        CheckGround();
        CheckWalls();
        HandleMovement();
        HandleJump();
        UpdateAnimatorParams();
    }

    // ---------------- CHECKS ----------------

    private void CheckFall()
    {
        if (transform.position.y >= fallThreshold)
            return;

        // p·d = ztr·ta ûivota
        if (health != null)
        {
            health.TakeDamage(1);

            // volitelnÈ: pokud chceö p¯i p·du zahr·t Hurt animaci
            if (anim != null)
                anim.SetTrigger(HurtHash);
        }
        else
        {
            ReturnToCheckpoint(); // fallback, kdyby Health nebyl na Playerovi
        }
    }

    private void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;

        // Pokud chceö mÌt "st·t na nep¯Ìteli = grounded", p¯idej i enemyLayer p¯es OR
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
        float moveInput = Input.GetAxisRaw("Horizontal"); // ost¯ejöÌ ovl·d·nÌ pro ploöinovku

        // blokace smÏru, pokud je stÏna
        if ((moveInput > 0 && wallRight) || (moveInput < 0 && wallLeft))
            moveInput = 0f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip hr·Ëe (jen kdyû se fakt h˝be)
        if (moveInput != 0f)
        {
            float scaleX = moveInput > 0f ? 1f : -1f;
            graphics.localScale = new Vector3(scaleX, 1, 1);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (anim != null)
                anim.SetTrigger("Jump");
        }
    }

    // ---------------- ANIMATION ----------------

    private void UpdateAnimatorParams()
    {
        if (anim == null)
            return;

        // Speed 0..1 podle re·lnÈ rychlosti (lepöÌ pro p¯echody Walk/Run)
        float speed01 = Mathf.Clamp01(Mathf.Abs(rb.linearVelocity.x) / moveSpeed);
        anim.SetFloat(SpeedHash, speed01);

        anim.SetBool(GroundedHash, isGrounded);

        // Dead se bude nastavovat ze systÈmu health (doporuËenÈ),
        // ale kdyby sis to chtÏl testovat, m˘ûeö sem doËasnÏ d·t logiku.
        // (Nech·v·m bez automatickÈho nastavov·nÌ, aù ti to nerozbije PlayerHealth.)
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
            : (spawnPoint != null ? (Vector2)spawnPoint.position : (Vector2)transform.position);
    }

    public void BackToSpawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint != null
            ? (Vector2)spawnPoint.position
            : Vector2.zero;
    }
}