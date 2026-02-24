using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    // Keys: podle ID (např. "blue_key", "lab_key_01")
    private readonly HashSet<string> keys = new HashSet<string>();

    [Header("Movement link (optional)")]
    [SerializeField] private PlayerMovement movement;

    private Coroutine speedBoostRoutine;

    private void Awake()
    {
        if (movement == null) movement = GetComponent<PlayerMovement>();
    }

    // ----- KEYS -----
    public void AddKey(string keyId)
    {
        if (!string.IsNullOrWhiteSpace(keyId))
            keys.Add(keyId);
    }

    public bool HasKey(string keyId) => keys.Contains(keyId);

    // ----- SPEED BOOST -----
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        if (movement == null) return;

        if (speedBoostRoutine != null)
            StopCoroutine(speedBoostRoutine);

        speedBoostRoutine = StartCoroutine(SpeedBoost(multiplier, duration));
    }

    private System.Collections.IEnumerator SpeedBoost(float multiplier, float duration)
    {
        movement.SpeedMultiplier *= multiplier;   // viz úprava PlayerMovement níž
        yield return new WaitForSeconds(duration);
        movement.SpeedMultiplier /= multiplier;
        speedBoostRoutine = null;
    }
}