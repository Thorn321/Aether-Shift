using UnityEngine;

public class DimensionObject : MonoBehaviour
{
    [SerializeField] private DimensionManager.Dimension visibleInDimension;
    private Renderer[] renderers;
    private Collider2D[] colliders;

    private void Awake()
    {
        // Load all renderers and colliders
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider2D>(true);
    }

    private void OnEnable()
    {
        if (DimensionManager.Instance != null)
        {
            DimensionManager.Instance.OnDimensionChanged += UpdateVisibility;
            UpdateVisibility(DimensionManager.Instance.currentDimension);
        }
    }

    private void OnDisable()
    {
        if (DimensionManager.Instance != null)
            DimensionManager.Instance.OnDimensionChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(DimensionManager.Dimension current)
    {
        bool isActive = (current == visibleInDimension);

        // Update visibility
        foreach (var r in renderers)
            r.enabled = isActive;

        foreach (var c in colliders)
            c.enabled = isActive;
    }
}
