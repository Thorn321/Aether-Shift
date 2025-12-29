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

    [Header("Dark Dimension Settings")]
    [SerializeField] private float maxDarkTime = 5f;
    private float darkTimer;

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

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Pøepnuto do DARK dimenze");
    }

    private void ReturnToLight()
    {
        currentDimension = Dimension.Light;

        if (mainCamera != null)
            mainCamera.backgroundColor = Color.cyan;

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Návrat do LIGHT dimenze");
    }

    private void ForceReturnToLight()
    {
        Debug.Log("Vypršel èas v temné dimenzi!");
        ReturnToLight();
        lastSwitchTime = Time.time;
    }
}
