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
    public float verticalBoost = 0.05f;

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
