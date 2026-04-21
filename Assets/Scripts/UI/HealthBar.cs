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

        // update při změně jazyka
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnHealthChanged -= HandleHealthChanged;

        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void Start()
    {
        if (player != null)
            HandleHealthChanged(player.CurrentLives, player.MaxLives);

        UpdateText();
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }

        UpdateText(current, max);
    }

    private void UpdateText()
    {
        if (player != null)
            UpdateText(player.CurrentLives, player.MaxLives);
    }

    private void UpdateText(int current, int max)
    {
        if (livesText != null && LocalizationManager.Instance != null)
        {
            string label = LocalizationManager.Instance.GetText("lives");
            livesText.text = $"{label}: {current} / {max}";
        }
    }
}