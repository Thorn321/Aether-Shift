using UnityEngine;

public class DimensionObject : MonoBehaviour
{
    [SerializeField] private DimensionManager.Dimension visibleInDimension;

    private Renderer[] renderers;
    private Collider2D[] colliders;
    private bool subscribed;

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
        // Záloha pro pøípad, že DimensionManager vznikne až po OnEnable
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    // ---------------- DIMENSION HANDLING ----------------

    private void TrySubscribe()
    {
        if (subscribed || DimensionManager.Instance == null)
            return;

        DimensionManager.Instance.OnDimensionChanged += UpdateVisibility;
        subscribed = true;

        UpdateVisibility(DimensionManager.Instance.currentDimension);
        Debug.Log($"{name}: Subscribed to DimensionManager. Current dimension: {DimensionManager.Instance.currentDimension}");
    }

    private void Unsubscribe()
    {
        if (!subscribed || DimensionManager.Instance == null)
            return;

        DimensionManager.Instance.OnDimensionChanged -= UpdateVisibility;
        subscribed = false;
    }

    // ---------------- VISIBILITY ----------------

    private void UpdateVisibility(DimensionManager.Dimension current)
    {
        bool isActive = current == visibleInDimension;

        SetRenderers(isActive);
        SetColliders(isActive);

        Debug.Log($"{name}: UpdateVisibility -> {current}. Active: {isActive}");
    }

    private void SetRenderers(bool state)
    {
        foreach (var r in renderers)
            r.enabled = state;
    }

    private void SetColliders(bool state)
    {
        foreach (var c in colliders)
            c.enabled = state;
    }
}
