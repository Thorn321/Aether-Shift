using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DarkDimensionEffect : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float transitionSpeed = 2f;

    private float targetWeight = 0f;

    private void Awake()
    {
        if (!volume)
            Debug.LogError("DarkDimensionEffect: Volume není pøiøazen!");
    }

    private void Update()
    {
        if (!volume) return;

        volume.weight = Mathf.MoveTowards(
            volume.weight,
            targetWeight,
            transitionSpeed * Time.deltaTime
        );
    }

    public void SetDark(bool dark)
    {
        targetWeight = dark ? 1f : 0f;
    }
}
