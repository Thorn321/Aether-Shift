using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DarkDimensionEffect : MonoBehaviour
{
    [SerializeField] private Material darkMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (DimensionManager.Instance != null &&
            DimensionManager.Instance.currentDimension == DimensionManager.Dimension.Dark)
        {
            Graphics.Blit(src, dest, darkMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}