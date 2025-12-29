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
    }

    private void CheckArrival()
    {
        if (Vector2.Distance(rb.position, currentTarget) < 0.1f)
            SwitchTarget();
    }

    private void SwitchTarget()
    {
        currentTarget = currentTarget == worldPointA ? worldPointB : worldPointA;
        Flip();
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}