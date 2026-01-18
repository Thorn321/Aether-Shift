using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [Header("Camera & Parallax Settings")]
    public Transform cam;
    public float parallaxX = 0.5f;
    public float maxHorizontalJump = 20f;
    public float moveSpeedX = 250f;

    [Header("Vertical")]
    [SerializeField] private float topOffset = 0.2f; // o kolik výš než vršek obrazovky (world units)

    private float startPosX;
    private float lengthX;
    private float heightY;
    private Camera camComp;

    private void Start()
    {
        startPosX = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        lengthX = sr.bounds.size.x;
        heightY = sr.bounds.size.y;

        camComp = Camera.main;
    }

    private void LateUpdate()
    {
        float targetX = startPosX + cam.position.x * parallaxX;
        float deltaX = cam.position.x * (1 - parallaxX);

        if (deltaX > startPosX + lengthX) startPosX += lengthX;
        else if (deltaX < startPosX - lengthX) startPosX -= lengthX;

        // Top edge zarovnat na top kamery + overscan
        float targetY = cam.position.y + camComp.orthographicSize - heightY / 2f + topOffset;

        Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);

        float horizontalDiff = Mathf.Abs(transform.position.x - targetX);
        if (horizontalDiff > maxHorizontalJump)
            transform.position = targetPos;
        else
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeedX * Time.deltaTime);
    }
}
