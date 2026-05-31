using System;
using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance { get; private set; }

    public enum Dimension { Light, Dark }
    public Dimension currentDimension { get; private set; } = Dimension.Light;

    public event Action<Dimension> OnDimensionChanged;

    [Header("Cooldown")]
    [SerializeField] private float switchCooldown = 0.8f;
    private float lastSwitchTime = -999f;

    [Header("Tilemaps")]
    [SerializeField] private GameObject tilemapLight;
    [SerializeField] private GameObject tilemapDark;

    [Header("Dark Dimension Settings")]
    [SerializeField] private float maxDarkTime = 5f;
    private float darkTimer;
    [SerializeField] private float darkAnimationSpeed = 0.5f;
    [SerializeField] private float lightAnimationSpeed = 1f;

    [Header("Camera Effects")]
    [SerializeField] private DarkDimensionEffect darkEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip switchToLightSound;
    [SerializeField] private AudioClip switchToDarkSound;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);

        mainCamera = Camera.main;
        UpdateTilemaps();
    }

    private void Update()
    {
        HandleInput();
        UpdateDarkTimer();
    }

    private void HandleInput()
    {
        if (!Input.GetKeyDown(KeyCode.F))
            return;

        if (Time.time - lastSwitchTime < switchCooldown)
            return;

        ToggleDimension();
        lastSwitchTime = Time.time;
    }

    private void UpdateDarkTimer()
    {
        if (currentDimension != Dimension.Dark)
            return;

        darkTimer -= Time.unscaledDeltaTime;

        if (darkTimer <= 0f)
            ForceReturnToLight();
    }

    private void ToggleDimension()
    {
        if (currentDimension == Dimension.Light)
            EnterDark();
        else
            ReturnToLight();
    }

    private void EnterDark()
    {
        currentDimension = Dimension.Dark;
        darkTimer = maxDarkTime;

        if (mainCamera != null)
            mainCamera.backgroundColor = Color.magenta;

        darkEffect.SetDark(true);
        UpdateTilemaps();
        ApplyAnimationSpeed(darkAnimationSpeed);

        if (switchToDarkSound != null)
            SFXManager.Instance.PlaySound(switchToDarkSound, 1f);

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Přepnuto do DARK dimenze");
    }

    private void ReturnToLight()
    {
        currentDimension = Dimension.Light;

        if (mainCamera != null)
            mainCamera.backgroundColor = Color.cyan;

        darkEffect.SetDark(false);
        UpdateTilemaps();
        ApplyAnimationSpeed(lightAnimationSpeed);

        if (switchToLightSound != null)
            SFXManager.Instance.PlaySound(switchToLightSound, 1f);

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Návrat do LIGHT dimenze");
    }

    public void ForceReturnToLight()
    {
        Debug.Log("Vypršel čas v temné dimenzi nebo si dostal damage!");
        ReturnToLight();
        lastSwitchTime = Time.time;
    }

    private void UpdateTilemaps()
    {
        if (tilemapLight != null)
            tilemapLight.SetActive(currentDimension == Dimension.Light);

        if (tilemapDark != null)
            tilemapDark.SetActive(currentDimension == Dimension.Dark);
    }

    private void ApplyAnimationSpeed(float speed)
    {
        Animator[] animators = FindObjectsOfType<Animator>();

        foreach (var anim in animators)
        {
            if (anim.gameObject.CompareTag("Player"))
                continue; 

            anim.speed = speed;
        }
    }
}