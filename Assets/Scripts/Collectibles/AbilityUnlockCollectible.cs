using UnityEngine;

public class AbilityUnlockCollectible : MonoBehaviour, ICollectible
{
    public enum AbilityType
    {
        DoubleJump,
        Dash
    }

    [Header("Ability")]
    [SerializeField] private AbilityType abilityToUnlock;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    [Header("Localization Keys")]
    [SerializeField] private string doubleJumpKey = "dialog_doublejump_unlocked";
    [SerializeField] private string dashKey = "dialog_dash_unlocked";

    public void Collect(PlayerCollector collector)
    {
        if (collector == null)
            return;

        PlayerAbilities abilities = collector.GetComponent<PlayerAbilities>();

        if (abilities == null)
            return;

        string dialogKey = "";

        switch (abilityToUnlock)
        {
            case AbilityType.DoubleJump:
                abilities.UnlockDoubleJump();
                dialogKey = doubleJumpKey;
                break;

            case AbilityType.Dash:
                abilities.UnlockDash();
                dialogKey = dashKey;
                break;
        }

        if (pickupSound != null)
        {
            SFXManager.Instance.PlaySound(pickupSound, 1);
        }

        if (DialogUI.Instance != null)
        {
            DialogUI.Instance.Show(dialogKey);
        }

        Destroy(gameObject);
    }
}