using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [Header("Camera & Parallax Settings")]
    public Transform cam;           // Cinemachine Virtual Camera
    public float parallaxX = 0.5f;  // horizontální parallax
    public float maxHorizontalJump = 20f; // pokud je rozdíl větší → okamžitý přesun
    public float moveSpeedX = 250f;  // horizontální rychlost dorovnání

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
        // Horizontální parallax
        float targetX = startPosX + cam.position.x * parallaxX;
        float deltaX = cam.position.x * (1 - parallaxX);

        if (deltaX > startPosX + lengthX) startPosX += lengthX;
        else if (deltaX < startPosX - lengthX) startPosX -= lengthX;

        // Vertikálně okamžitě podle kamery
        float targetY = cam.position.y + camComp.orthographicSize - heightY / 2f;

        Vector3 targetPos = new Vector3(targetX, targetY, transform.position.z);

        // Horizontální pohyb - pokud je rozdíl obrovský, nastavíme rovnou
        float horizontalDiff = Mathf.Abs(transform.position.x - targetX);
        if (horizontalDiff > maxHorizontalJump)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(targetX, targetY, transform.position.z),
                moveSpeedX * Time.deltaTime);
        }
    }
}
