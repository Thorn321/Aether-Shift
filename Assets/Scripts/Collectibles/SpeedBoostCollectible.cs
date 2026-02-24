using UnityEngine;

public class SpeedBoostCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private float multiplier = 1.5f;
    [SerializeField] private float duration = 3f;

    public void Collect(PlayerCollector collector)
    {
        collector.ApplySpeedBoost(multiplier, duration);
        Destroy(gameObject);
    }
}