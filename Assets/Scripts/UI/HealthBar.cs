using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth player;
    [SerializeField] private Slider slider;

    private void Start()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (player != null)
        {
            slider.maxValue = player.MaxLives;
            slider.value = player.CurrentLives;
        }
    }

    public void UpdateHealth(int current)
    {
        slider.value = current;
    }
}
