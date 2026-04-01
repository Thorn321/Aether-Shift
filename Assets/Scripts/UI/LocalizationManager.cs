using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedText;

    public static event Action OnLanguageChanged; // static event → může být registrovaný kdykoliv

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            string lang = PlayerPrefs.GetString("Language", "en");
            LoadLanguage(lang);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLanguage(string lang)
    {
        string path = Path.Combine(Application.streamingAssetsPath, $"lang_{lang}.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LocalizationData data = JsonUtility.FromJson<LocalizationData>(json);

            localizedText = new Dictionary<string, string>();
            foreach (var item in data.items)
                localizedText[item.key] = item.value;

            Debug.Log("Language loaded: " + lang);

            // Nové: spustí všechny registrované texty
            OnLanguageChanged?.Invoke();
        }
        else
        {
            Debug.LogError("Language file not found: " + path);
        }
    }

    public string GetText(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
            return localizedText[key];
        return key;
    }
}

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}