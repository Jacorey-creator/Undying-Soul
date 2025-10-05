using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image glowImage;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color bonusGlowColor = Color.cyan;

    private PlayerHealthController targetHealth;

    private void OnEnable()
    {
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
            UpdateHealthUI(0f, 1f);
        }
    }

    private void UpdateHealthUI(float current, float max)
    {
        if (healthFillImage == null)
            return;

        float fill = Mathf.Clamp01(current / max);
        healthFillImage.fillAmount = fill;

        // === Over-heal ===
        if (current > max)
        {
            glowImage.gameObject.SetActive(true);
            glowImage.color = bonusGlowColor;
            healthFillImage.color = bonusGlowColor;
            return;
        }

        glowImage.gameObject.SetActive(false);

        // === Normal & Low Health Color Transition ===
        float healthPercent = current / max;

        if (healthPercent <= 0.2f)
        {
            // Gradually blend from green -> red as health approaches 0%
            float t = Mathf.InverseLerp(0.2f, 0f, healthPercent);
            Color blended = Color.Lerp(normalColor, lowHealthColor, t);
            healthFillImage.color = blended;
        }
        else
        {
            // Above 20% health, normal color
            healthFillImage.color = normalColor;
        }
    }
}
