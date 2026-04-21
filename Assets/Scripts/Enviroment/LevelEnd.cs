using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public GameObject levelCompleteUI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            levelCompleteUI.SetActive(true);

            // zastaví hráče
            Time.timeScale = 0f;
        }
    }
}
