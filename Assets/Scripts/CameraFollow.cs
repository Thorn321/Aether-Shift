using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = new Vector3(
                player.position.x + offset.x,
                player.position.y + offset.y,
                transform.position.z
            );
        }
    }


    // SMOOTH FOLLOW
    /* public Transform player;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }*/
}
