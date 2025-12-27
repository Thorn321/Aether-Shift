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

        // Pøihlášení na zmìnu dimenze
        if (DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnDimensionChanged += OnDimensionChanged;
            OnDimensionChanged(DimensionManager.Instance.currentDimension);
        }
    }

    protected virtual void OnDestroy()
    {
        if (DimensionManager.Instance != null)
            DimensionManager.Instance.OnDimensionChanged -= OnDimensionChanged;
    }

    protected void OnDimensionChanged(DimensionManager.Dimension dimension)
    {
        timeMultiplier =
            (dimension == DimensionManager.Dimension.Dark)
            ? darkTimeMultiplier
            : 1f;
    }

    protected abstract void Move();

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HitPlayer(collision.gameObject);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HitPlayer(collision.gameObject);
        }
    }

    private void HitPlayer(GameObject player)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        Debug.Log($"Enemy hit player! (Enemy: {gameObject.name})");

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.ReturnToCheckpoint();
        }
    }
}
