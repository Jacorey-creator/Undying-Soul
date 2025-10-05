using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image glowImage;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color bonusGlowColor = Color.cyan;

    private PlayerHealthController targetHealth;

    private void OnEnable()
    {
        // If a target is already assigned (e.g., from PossessionManager), hook into it
        if (targetHealth != null)
            targetHealth.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateHealthUI;
    }

    /// <summary>
    /// Assigns which player's health this UI should track.
    /// </summary>
    public void SetTarget(PlayerHealthController newTarget)
    {
        // Unsubscribe from the previous target (if any)
        if (targetHealth != null)
            targetHealth.OnHealthChanged -= UpdateHealthUI;

        targetHealth = newTarget;

        if (targetHealth != null)
        {
            targetHealth.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(targetHealth.CurrentHealth, targetHealth.MaxHealth);
        }
        else
        {
            // Clear UI if no player
            UpdateHealthUI(0f, 1f);
        }
    }

    private void UpdateHealthUI(float current, float max)
    {
        if (healthFillImage == null)
            return;

        float fill = Mathf.Clamp01(current / max);
        healthFillImage.fillAmount = fill;

        // Change color based on over-heal state
        if (current > max)
        {
            glowImage.gameObject.SetActive(true);
            glowImage.color = bonusGlowColor;
            healthFillImage.color = bonusGlowColor;
        }
        else
        {
            glowImage.gameObject.SetActive(false);
            healthFillImage.color = normalColor;
        }
    }
}
