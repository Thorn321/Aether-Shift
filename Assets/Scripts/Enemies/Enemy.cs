using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    public float moveSpeed = 2f;

    [Header("Damage Settings")]
    public int damage = 1;
    public float damageCooldown = 1f;

    [Header("Time Settings")]
    [SerializeField] protected float darkTimeMultiplier = 0.5f;

    protected Rigidbody2D rb;
    protected bool movingRight = true;
    protected float timeMultiplier = 1f;

    private float lastDamageTime;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SubscribeToDimension();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromDimension();
    }

    // ---------------- DIMENSION HANDLING ----------------

    private void SubscribeToDimension()
    {
        if (DimensionManager.Instance == null)
            return;

        DimensionManager.Instance.OnDimensionChanged += OnDimensionChanged;
        OnDimensionChanged(DimensionManager.Instance.currentDimension);
    }

    private void UnsubscribeFromDimension()
    {
        if (DimensionManager.Instance == null)
            return;

        DimensionManager.Instance.OnDimensionChanged -= OnDimensionChanged;
    }

    protected void OnDimensionChanged(DimensionManager.Dimension dimension)
    {
        timeMultiplier = (dimension == DimensionManager.Dimension.Dark)
            ? darkTimeMultiplier
            : 1f;
    }

    // ---------------- MOVEMENT ----------------

    protected abstract void Move();

    protected virtual void FixedUpdate()
    {
        Move();
    }

    // ---------------- DAMAGE ----------------

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        TryHitPlayer(collision.collider);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        TryHitPlayer(collision.collider);
    }

    private void TryHitPlayer(Collider2D collider)
    {
        if (!collider.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        Debug.Log($"Enemy hit player! (Enemy: {name})");

        PlayerMovement player = collider.GetComponent<PlayerMovement>();
        if (player != null)
            player.ReturnToCheckpoint();
    }
}
