using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform targetTeleport;

    private static Teleporter lastTeleporter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // zabrání okamžitému návratu
        if (lastTeleporter == this) return;

        // teleport
        other.transform.position = targetTeleport.position;

        // uloží poslední teleporter
        lastTeleporter = targetTeleport.GetComponent<Teleporter>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (lastTeleporter == this)
        {
            lastTeleporter = null;
        }
    }
}