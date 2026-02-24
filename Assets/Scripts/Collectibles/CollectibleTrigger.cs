using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    private PlayerCollector collector;

    private void Awake()
    {
        collector = GetComponentInParent<PlayerCollector>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collector == null) return;

        // Zkus najít collectible komponentu na objektu (nebo parentovi)
        var collectible = other.GetComponent<ICollectible>();
        if (collectible == null)
            collectible = other.GetComponentInParent<ICollectible>();

        collectible?.Collect(collector);
    }
}