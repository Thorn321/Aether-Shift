using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxLives = 5;
    public int MaxLives => maxLives;
    public int CurrentLives { get; private set; }
    [SerializeField] private HealthBar healthUI;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime = 0.8f;
    private float lastHitTime = -999f;

    [Header("Animation Timings")]
    [SerializeField] private float hurtAnimDelay = 1f;
    [SerializeField] private float deathAnimDelay = 5f;

    private PlayerMovement movement;
    private Animator anim;
    private Rigidbody2D rb;

    private bool isDead;

    // Animator hashes
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Death");

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (healthUI == null)
            healthUI = Object.FindFirstObjectByType<HealthBar>();

        CurrentLives = maxLives;
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
            return;

        if (Time.time - lastHitTime < invincibilityTime)
            return;

        lastHitTime = Time.time;

        // 2× damage v Dark dimenzi + návrat do Light
        if (DimensionManager.Instance != null &&
            DimensionManager.Instance.currentDimension == DimensionManager.Dimension.Dark)
        {
            amount *= 2;
            DimensionManager.Instance.ForceReturnToLight();
        }

        CurrentLives = Mathf.Clamp(CurrentLives - amount, 0, maxLives);
        if (healthUI != null)
            healthUI.UpdateHealth(CurrentLives);
        Debug.Log($"Player took damage: {amount}. Lives: {CurrentLives}/{maxLives}");

        if (CurrentLives <= 0)
        {
            StartCoroutine(DieRoutine());
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger(HurtHash);

        yield return new WaitForSeconds(hurtAnimDelay);

        if (movement != null)
            movement.ReturnToCheckpoint();
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Death trigger
        if (anim != null)
            anim.SetTrigger(DieHash);

        yield return new WaitForSeconds(deathAnimDelay);

        // pojistka – zamkne poslední frame
        if (anim != null)
            anim.enabled = false;

        Debug.Log("Player died -> restart level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
