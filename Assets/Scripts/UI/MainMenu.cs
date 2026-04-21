using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject background;

    [Header("Audio")]
    public AudioMixer mixer;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string LANGUAGE_KEY = "Language";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        background.SetActive(true);

        // načti uložená nastavení
        LoadSettings();

        // posluchače sliderů
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            mainMenuPanel.SetActive(true);
            optionsPanel.SetActive(false);
            background.SetActive(true);
        }
    }

    // ---------------- MENU ----------------

    public void OnStartButton()
    {
        Debug.Log("Start button kliknut!");
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        background.SetActive(false);
        SceneManager.LoadScene("Level1");
    }

    public void OnQuitButton()
    {
        Debug.Log("Quit button kliknut!");
        Application.Quit();
    }

    public void OnOptionsButton()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ---------------- AUDIO ----------------

    // slider 0-100 → převedeno na 0-1 → dB
    public void SetMasterVolume(float value)
    {
        float normalized = Mathf.Clamp(value / 100f, 0.0001f, 1f);
        mixer.SetFloat("MasterVolume", Mathf.Log10(normalized) * 20);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        float normalized = Mathf.Clamp(value / 100f, 0.0001f, 1f);
        mixer.SetFloat("SFXVolume", Mathf.Log10(normalized) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        float normalized = Mathf.Clamp(value / 100f, 0.0001f, 1f);
        mixer.SetFloat("MusicVolume", Mathf.Log10(normalized) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    // ---------------- LANGUAGE ----------------

    public void SetEnglish()
    {
        PlayerPrefs.SetString(LANGUAGE_KEY, "en");
        LocalizationManager.Instance.LoadLanguage("en");
    }

    public void SetCzech()
    {
        PlayerPrefs.SetString(LANGUAGE_KEY, "cz");
        LocalizationManager.Instance.LoadLanguage("cz");
    }

    // ---------------- LOAD ----------------

    void LoadSettings()
    {
        // načti audio
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 100f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 100f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 100f);

        masterSlider.value = master;
        sfxSlider.value = sfx;
        musicSlider.value = music;

        SetMasterVolume(master);
        SetSFXVolume(sfx);
        SetMusicVolume(music);

        // načti jazyk
        string lang = PlayerPrefs.GetString(LANGUAGE_KEY, "en");
        Debug.Log("Loaded language: " + lang);
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.LoadLanguage(lang);
    }
}