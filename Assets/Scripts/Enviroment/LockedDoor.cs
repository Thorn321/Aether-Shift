using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private string requiredKeyId = "blue_key";
    [SerializeField] private GameObject doorVisual;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private bool destroyOnUnlock = false;

    private bool unlocked;

    private void Awake()
    {
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();
        if (doorVisual == null) doorVisual = gameObject;
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

        if (destroyOnUnlock)
        {
            Destroy(gameObject);
            return;
        }

        if (doorCollider != null) doorCollider.enabled = false;
        if (doorVisual != null) doorVisual.SetActive(false);
    }
}