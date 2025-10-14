using UnityEngine;

public class EnemyWalking : Enemy
{
    [Header("Ground & Wall Check")]
    public LayerMask groundLayer;
    public float wallCheckDistance = 0.2f;
    public Vector2 wallCheckBottomOffset = new Vector2(0, -0.4f);
    public Vector2 wallCheckTopOffset = new Vector2(0, 0.4f);

    protected override void Move()
    {
        // Pohyb pouze horizontálně
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);

        // Kontrola okrajů a zdí
        CheckGroundAndWalls();
    }

    private void Update()
    {
        Move();
    }

    private void CheckGroundAndWalls()
    {
        Vector2 pos = transform.position;

        float direction = movingRight ? 1f : -1f;
        Vector2 bottomFront = pos + new Vector2(0.5f * direction, -0.5f); // přední noha
        Vector2 bottomBack = pos + new Vector2(-0.5f * direction, -0.5f); // zadní noha

        // Ray dolů u přední nohy
        float groundRayLength = 0.1f;
        RaycastHit2D groundRay = Physics2D.Raycast(bottomFront, Vector2.down, groundRayLength, groundLayer);

        // Ray dopředu
        float wallRayLength = 0.1f;
        RaycastHit2D wallRayBottom = Physics2D.Raycast(bottomFront, Vector2.right * direction, wallRayLength, groundLayer);
        RaycastHit2D wallRayTop = Physics2D.Raycast(pos + new Vector2(0.5f * direction, 0.5f), Vector2.right * direction, wallRayLength, groundLayer);

        Debug.DrawRay(bottomFront, Vector2.down * groundRayLength, Color.green);
        Debug.DrawRay(bottomFront, Vector2.right * direction * wallRayLength, Color.red);
        Debug.DrawRay(pos + new Vector2(0.5f * direction, 0.5f), Vector2.right * direction * wallRayLength, Color.red);

        // Flip, pokud není zem přední nohou nebo je zeď
        if (groundRay.collider == null || wallRayBottom.collider != null || wallRayTop.collider != null)
        {
            Flip();
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
