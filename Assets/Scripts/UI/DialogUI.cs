using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public static DialogUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject window;
    [SerializeField] private UIText messageText;
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(Hide);
        }

        // důležité: začínáme disabled stavem
        if (window != null)
            window.SetActive(false);
    }

    public void Show(string localizationKey)
    {
        if (window != null)
            window.SetActive(true);

        if (messageText != null)
        {
            messageText.key = localizationKey;
            messageText.UpdateText();
        }

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (window != null)
            window.SetActive(false);

        Time.timeScale = 1f;
    }
}