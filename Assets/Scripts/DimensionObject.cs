using UnityEngine;

public class DimensionObject : MonoBehaviour
{
    [SerializeField] private DimensionManager.Dimension visibleInDimension;
    private Renderer[] renderers;
    private Collider2D[] colliders;
    private bool subscribed = false;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        // Pokud OnEnable bìžel døív než DimensionManager vznikl, Start zajistí pøihlášení
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (subscribed && DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnDimensionChanged -= UpdateVisibility;
            subscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (subscribed) return;

        if (DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnDimensionChanged += UpdateVisibility;
            subscribed = true;
            UpdateVisibility(DimensionManager.Instance.currentDimension);
            Debug.Log($"{name}: Subscribed to DimensionManager. Current dimension: {DimensionManager.Instance.currentDimension}");
        }
    }

    private void UpdateVisibility(DimensionManager.Dimension current)
    {
        bool isActive = (current == visibleInDimension);

        foreach (var r in renderers)
            r.enabled = isActive;

        foreach (var c in colliders)
            c.enabled = isActive;

        Debug.Log($"{name}: UpdateVisibility -> {current}. Renderers enabled: {isActive}");
    }
}
