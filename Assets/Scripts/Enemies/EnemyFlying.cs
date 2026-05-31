using UnityEngine;

public class EnemyFlying : Enemy
{
    [Header("Flying Path")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    private Vector2 worldPointA;
    private Vector2 worldPointB;
    private Vector2 currentTarget;

    private void Awake()
    {
        if (!sr)
            sr = GetComponentInChildren<SpriteRenderer>();

        if (!anim)
            anim = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();
        if (!ValidatePoints()) return;

        InitializeWorldPoints();
        currentTarget = worldPointB;
    }

    protected override void Move()
    {
        MoveTowardsTarget();
        CheckArrival();
    }

    // ---------------- INIT ----------------

    private bool ValidatePoints()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"{name}: EnemyFlying requires both pointA and pointB!");
            enabled = false;
            return false;
        }
        return true;
    }

    private void InitializeWorldPoints()
    {
        worldPointA = pointA.position;
        worldPointB = pointB.position;
    }

    // ---------------- MOVEMENT ----------------

    private void MoveTowardsTarget()
    {
        Vector2 position = rb.position;
        Vector2 direction = (currentTarget - position).normalized;

        rb.linearVelocity = direction * moveSpeed * timeMultiplier;

        HandleFlip(direction);
    }

    private void CheckArrival()
    {
        if (Vector2.Distance(rb.position, currentTarget) < 0.15f)
            SwitchTarget();
    }

    private void SwitchTarget()
    {
        currentTarget = (currentTarget == worldPointA) ? worldPointB : worldPointA;
    }

    // ---------------- VISUAL ----------------

    private void HandleFlip(Vector2 direction)
    {
        if (!sr) return;

        if (Mathf.Abs(direction.x) > 0.01f)
            sr.flipX = direction.x < 0;
    }
}