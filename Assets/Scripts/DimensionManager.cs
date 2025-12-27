using System;
using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance { get; private set; }

    public enum Dimension { Light, Dark }
    public Dimension currentDimension { get; private set; } = Dimension.Light;

    public event Action<Dimension> OnDimensionChanged;

    [Header("Cooldown")]
    [SerializeField] private float switchCooldown = 0.5f;
    private float lastSwitchTime = -999f;

    [Header("Dark Dimension Settings")]
    [SerializeField] private float maxDarkTime = 5f;

    private float darkTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time - lastSwitchTime >= switchCooldown)
            {
                ToggleDimension();
                lastSwitchTime = Time.time;
            }
        }

        if (currentDimension == Dimension.Dark)
        {
            darkTimer -= Time.unscaledDeltaTime;

            if (darkTimer <= 0f)
                ForceReturnToLight();
        }
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

        if (Camera.main != null)
            Camera.main.backgroundColor = Color.magenta;

        OnDimensionChanged?.Invoke(currentDimension);
        Debug.Log("Pøepnuto do DARK dimenze");
    }

    private void ReturnToLight()
    {
        currentDimension = Dimension.Light;

        if (Camera.main != null)
            Camera.main.backgroundColor = Color.cyan;

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
