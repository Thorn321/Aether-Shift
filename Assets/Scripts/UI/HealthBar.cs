using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text livesText;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (player == null)
            player = Object.FindFirstObjectByType<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (player != null)
            player.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        if (player != null)
            HandleHealthChanged(player.CurrentLives, player.MaxLives);
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }

        if (livesText != null)
            livesText.text = $"Lives: {current} / {max}";
    }
}