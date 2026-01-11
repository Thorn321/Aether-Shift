using UnityEngine;

public class FitSpriteOnCamera : MonoBehaviour
{
    private void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("BackgroundScaler: SpriteRenderer nebo Sprite chybí!");
            return;
        }

        // Velikost sprite ve svìtových jednotkách
        float spriteWidthUnits = sr.sprite.texture.width / sr.sprite.pixelsPerUnit;
        float spriteHeightUnits = sr.sprite.texture.height / sr.sprite.pixelsPerUnit;

        // Velikost kamery ve svìtových jednotkách
        Camera cam = Camera.main;
        float cameraHeightUnits = cam.orthographicSize * 2f;
        float cameraWidthUnits = cameraHeightUnits * cam.aspect;

        // Scale pro zmenšení sprite, aby se vešel na obrazovku
        float scaleX = cameraWidthUnits / spriteWidthUnits;
        float scaleY = cameraHeightUnits / spriteHeightUnits;
        float scale = Mathf.Min(scaleX, scaleY); // zachování pomìru stran

        // Nastavení scale
        transform.localScale = new Vector3(scale, scale, 1f);

        Debug.Log($"BackgroundScaler: Scale nastaven na {scale}");
    }
}
