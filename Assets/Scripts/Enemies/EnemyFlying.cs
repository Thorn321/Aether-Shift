using UnityEngine;

public class EnemyFlying : Enemy
{
    [Header("Flying Path")]
    public Transform pointA;
    public Transform pointB;

    private Vector2 worldPointA;
    private Vector2 worldPointB;
    private Vector2 currentTarget;

    protected override void Start()
    {
        base.Start();

        if (pointA == null || pointB == null)
        {
            Debug.LogError($"{name}: FlyingEnemy needs pointA and pointB!");
            enabled = false;
            return;
        }

        // Uložíme si world pozice (nezávislé na parentovi)
        worldPointA = pointA.position;
        worldPointB = pointB.position;

        currentTarget = worldPointB;
    }

    protected override void Move()
    {
        Vector2 position = rb.position;
        Vector2 direction = (currentTarget - position).normalized;

        rb.linearVelocity = direction * moveSpeed * timeMultiplier;

        if (Vector2.Distance(position, currentTarget) < 0.1f)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        currentTarget = (currentTarget == worldPointA)
            ? worldPointB
            : worldPointA;

        Flip();
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}