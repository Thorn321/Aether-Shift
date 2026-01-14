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

    [Header("Camera Effects")]
    [SerializeField] private DarkDimensionEffect darkEffect;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

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
        if (!Input.GetKeyDown(KeyCode.E))
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

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Návrat do LIGHT dimenze");
    }

    private void ForceReturnToLight()
    {
        Debug.Log("Vypršel čas v temné dimenzi!");
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
}