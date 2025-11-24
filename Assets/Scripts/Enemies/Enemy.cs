using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    public float moveSpeed = 2f;

    [Header("Damage Settings")]
    public int damage = 1;
    public float damageCooldown = 1f;

    [Header("Knockback Settings")]
    public float knockbackForce = 0.3f;
    public float verticalBoost = 0.2f;

    protected Rigidbody2D rb;
    protected bool movingRight = true;
    private float lastDamageTime;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected abstract void Move();

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TryDamagePlayer(collision.gameObject);
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TryDamagePlayer(collision.gameObject);
        }
    }

    private void TryDamagePlayer(GameObject player)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;

        Debug.Log($"Enemy hit player! Damage: {damage} (Enemy: {gameObject.name})");

        // Smìr pryè od enemy
        Vector2 knockDir = (player.transform.position - transform.position).normalized;
        knockDir.y = 0; // horizontální knockback
        knockDir = -knockDir; // pryè od enemy
        knockDir.y = verticalBoost; // pøidáme vertikální boost

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.ApplyKnockback(knockDir, knockbackForce);
    }

}
