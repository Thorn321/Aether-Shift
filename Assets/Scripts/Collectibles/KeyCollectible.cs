using UnityEngine;

public class KeyCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private string keyId = "blue_key";
    [SerializeField] private AudioSource sfx; // volitelné

    public void Collect(PlayerCollector collector)
    {
        collector.AddKey(keyId);

        if (sfx != null) sfx.Play();

        // jednoduché: znič hned
        Destroy(gameObject);
    }
}