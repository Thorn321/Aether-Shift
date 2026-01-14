using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // Uložení checkpointu
                player.SetCheckpoint(transform.position);
                activated = true;

                Debug.Log("Checkpoint uložen: " + transform.position);

                Effects();
            }
        }
    }

    private void Effects()
    {
        if (sr != null)
        {
            sr.color = new Color(0.5f, 1f, 0.5f);
        }
    }
}
