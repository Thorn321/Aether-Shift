using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [Header("Camera & Parallax Settings")]
    [SerializeField] private Transform cam;

    [SerializeField] private float parallaxX = 0.5f;

    [SerializeField] private float maxHorizontalJump = 20f;
    [SerializeField] private float moveSpeedX = 250f;

    [Header("Vertical")]
    [SerializeField] private float topOffset = 0f;

    private float startPosX;
    private float spriteWidth;
    private float spriteHeight;

    private Camera camComp;

    private void Start()
    {
        camComp = Camera.main;

        if (cam == null && camComp != null)
            cam = camComp.transform;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        spriteWidth = sr.bounds.size.x;
        spriteHeight = sr.bounds.size.y;

        startPosX = transform.position.x;
    }

    private void LateUpdate()
    {
        if (cam == null || camComp == null)
            return;

        // =========================
        // PARALLAX X
        // =========================

        float targetX = startPosX + cam.position.x * parallaxX;
        float deltaX = cam.position.x * (1f - parallaxX);

        if (deltaX > startPosX + spriteWidth)
            startPosX += spriteWidth;
        else if (deltaX < startPosX - spriteWidth)
            startPosX -= spriteWidth;

        // =========================
        // TOP ALIGN
        // =========================

        float cameraTop =
            cam.position.y + camComp.orthographicSize;

        float targetY =
            cameraTop - (spriteHeight * 0.5f) + topOffset;

        Vector3 targetPos =
            new Vector3(targetX, targetY, transform.position.z);

        // =========================
        // SNAP / SMOOTH MOVE
        // =========================

        float horizontalDiff =
            Mathf.Abs(transform.position.x - targetX);

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
}