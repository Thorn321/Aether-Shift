using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cam;
    private Camera camComp;

    [Header("Parallax")]
    [SerializeField] private float parallaxX = 0.5f;

    [Header("Movement")]
    [SerializeField] private float maxHorizontalJump = 20f;
    [SerializeField] private float moveSpeedX = 250f;

    [Header("Vertical")]
    [SerializeField] private float verticalOffset = 0f;

    [Header("Auto Scale")]
    [SerializeField] private bool autoScaleY = true;
    [SerializeField] private float extraScaleMargin = 1.2f;

    private SpriteRenderer sr;

    private float startPosX;
    private float spriteWidth;

    private float baseSpriteHeight;

    private void Awake()
    {
        camComp = Camera.main;

        if (cam == null && camComp != null)
            cam = camComp.transform;

        sr = GetComponent<SpriteRenderer>();

        // IMPORTANT: čistá velikost sprite bez scale
        baseSpriteHeight = sr.sprite.bounds.size.y;
        spriteWidth = sr.sprite.bounds.size.x;

        startPosX = transform.position.x;
    }

    private void LateUpdate()
    {
        if (cam == null || camComp == null || sr == null)
            return;

        ApplyAutoScale();

        HandleParallaxX();
        HandleVertical();
    }

    private void ApplyAutoScale()
    {
        if (!autoScaleY) return;

        float cameraHeight = camComp.orthographicSize * 2f;

        float targetScaleY =
            (cameraHeight / baseSpriteHeight) * extraScaleMargin;

        Vector3 scale = transform.localScale;
        scale.y = targetScaleY;
        transform.localScale = scale;
    }

    private void HandleParallaxX()
    {
        float targetX = startPosX + cam.position.x * parallaxX;

        float deltaX = cam.position.x * (1f - parallaxX);

        if (deltaX > startPosX + spriteWidth)
            startPosX += spriteWidth;
        else if (deltaX < startPosX - spriteWidth)
            startPosX -= spriteWidth;

        float horizontalDiff =
            Mathf.Abs(transform.position.x - targetX);

        Vector3 targetPos = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        if (horizontalDiff > maxHorizontalJump)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeedX * Time.deltaTime
            );
        }
    }

    private void HandleVertical()
    {
        transform.position = new Vector3(
            transform.position.x,
            cam.position.y + verticalOffset,
            transform.position.z
        );
    }
}