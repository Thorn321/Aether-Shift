using UnityEngine;

public class EnemyFlying : Enemy
{
    [Header("Flying Path")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private Vector2 worldPointA;
    private Vector2 worldPointB;
    private Vector2 currentTarget;

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

    // ---------------- INITIALIZATION ----------------

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
        Flip(direction);
    }

    private void CheckArrival()
    {
        if (Vector2.Distance(rb.position, currentTarget) < 0.1f)
            SwitchTarget();
    }

    private void SwitchTarget()
    {
        currentTarget = currentTarget == worldPointA ? worldPointB : worldPointA;
    }

    private void Flip(Vector2 direction)
    {
        if (direction.x == 0) return;

        float scaleX = direction.x > 0 ? 1f : -1f;
        transform.localScale = new Vector3(scaleX, 1f, 1f);
    }
}