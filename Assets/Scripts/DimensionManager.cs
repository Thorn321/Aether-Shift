using System;
using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance { get; private set; }

    public enum Dimension { Light, Dark }
    public Dimension currentDimension = Dimension.Light;

    public event Action<Dimension> OnDimensionChanged;

    [Header("Cooldown")]
    [SerializeField] public float switchCooldown = 0.5f;
    private float lastSwitchTime = -999f;

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
        // Pokus o pøepnutí dimenze
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time - lastSwitchTime >= switchCooldown)
            {
                ToggleDimension();
                lastSwitchTime = Time.time;
            }
            else
            {
                // Debug.Log("Cooldown - nemùžeš pøepnout ještì!");
            }
        }
    }

    private void ToggleDimension()
    {
        currentDimension = currentDimension == Dimension.Light ? Dimension.Dark : Dimension.Light;
        Debug.Log("Pøepnuto na dimenzi: " + currentDimension);

        OnDimensionChanged?.Invoke(currentDimension);

        Camera.main.backgroundColor =
            (currentDimension == Dimension.Light) ? Color.cyan : Color.magenta;
    }
}
