using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private string requiredKeyId = "blue_key";
    [SerializeField] private GameObject doorVisual;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private bool destroyOnUnlock = false;

    [Header("Animation")]
    [SerializeField] private float moveDownDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;

    private bool unlocked;
    private bool isAnimating;

    private Vector3 startPos;
    private Vector3 targetPos;

    private void Awake()
    {
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();
        if (doorVisual == null) doorVisual = gameObject;
    }

    private void Update()
    {
        if (!isAnimating) return;

        Debug.Log("Animating door...");
        doorVisual.transform.position = Vector3.MoveTowards(
            doorVisual.transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // když dojede dolů
        if (Vector3.Distance(doorVisual.transform.position, targetPos) < 0.01f)
        {
            isAnimating = false;

            if (destroyOnUnlock)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    // Ze sensoru
    public void TryUnlock(Collider2D other)
    {
        if (unlocked) return;

        var collector = other.GetComponentInParent<PlayerCollector>();
        if (collector == null) return;

        if (collector.HasKey(requiredKeyId))
            Unlock();
    }

    private void Unlock()
    {
        unlocked = true;

        if (doorCollider != null)
            doorCollider.enabled = false;

        // nastav animaci
        startPos = doorVisual.transform.position;
        targetPos = startPos + Vector3.down * moveDownDistance;

        isAnimating = true;
    }
}