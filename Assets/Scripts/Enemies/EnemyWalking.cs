using UnityEngine;

public class EnemyWalking : Enemy
{
    [Header("Ground & Wall Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private float horizontalOffset = 0.5f;
    [SerializeField] private float verticalOffset = 0.5f;

    protected override void Move()
    {
        MoveHorizontally();
        CheckGroundAndWalls();
    }

    // ---------------- MOVEMENT ----------------

    private void MoveHorizontally()
    {
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed * timeMultiplier, rb.linearVelocity.y);
    }

    // ---------------- ENVIRONMENT CHECK ----------------

    private void CheckGroundAndWalls()
    {
        float direction = movingRight ? 1f : -1f;
        Vector2 position = transform.position;

        Vector2 frontBottom = position + new Vector2(horizontalOffset * direction, -verticalOffset);
        Vector2 frontTop = position + new Vector2(horizontalOffset * direction, verticalOffset);

        RaycastHit2D groundHit = Physics2D.Raycast(frontBottom, Vector2.down, rayLength, groundLayer);
        RaycastHit2D wallHitBottom = Physics2D.Raycast(frontBottom, Vector2.right * direction, rayLength, groundLayer);
        RaycastHit2D wallHitTop = Physics2D.Raycast(frontTop, Vector2.right * direction, rayLength, groundLayer);

        DrawDebugRays(frontBottom, frontTop, direction);

        if (groundHit.collider == null || wallHitBottom.collider != null || wallHitTop.collider != null)
        {
            Flip();
        }
    }

    // ---------------- VISUAL DEBUG ----------------

    private void DrawDebugRays(Vector2 bottom, Vector2 top, float direction)
    {
        Debug.DrawRay(bottom, Vector2.down * rayLength, Color.green);
        Debug.DrawRay(bottom, Vector2.right * direction * rayLength, Color.red);
        Debug.DrawRay(top, Vector2.right * direction * rayLength, Color.red);
    }

    // ---------------- DIRECTION ----------------

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
