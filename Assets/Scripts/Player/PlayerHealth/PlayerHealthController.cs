using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealthController : MonoBehaviour
{
    private PlayerController playerController;
    private float currentHealth;
    private float maxHealth;

    public event Action<float, float> OnHealthChanged; // (current, max)
    public event Action OnDeath;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource lowhealth_audioSource;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip lowhealthClip;

    private bool isLowHealthActive = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerHealthController requires a PlayerController!");
            return;
        }

        maxHealth = playerController.GetMaxHealth();
        currentHealth = maxHealth;

        if (lowhealth_audioSource != null)
        {
            lowhealth_audioSource.clip = lowhealthClip;
            lowhealth_audioSource.loop = true;
            lowhealth_audioSource.playOnAwake = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        AudioHelper.PlaySound(damageClip, audioSource);

        HandleLowHealthAudio();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        HandleLowHealthAudio();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void HandleLowHealthAudio()
    {
        if (lowhealth_audioSource == null || lowhealthClip == null) return;

        float healthPercent = currentHealth / maxHealth;

        // If below 20%, start or adjust the low health audio
        if (healthPercent <= 0.20f && currentHealth > 0)
        {
            if (!lowhealth_audioSource.isPlaying)
            {
                lowhealth_audioSource.Play();
                isLowHealthActive = true;
            }

            // Adjust pitch based on severity
            if (healthPercent <= 0.10f)
                lowhealth_audioSource.pitch = 1.5f; // faster heartbeat
            else
                lowhealth_audioSource.pitch = 1.0f; // normal speed
        }
        else if (isLowHealthActive && healthPercent > 0.15f)
        {
            // Fade out or stop when health is above threshold
            lowhealth_audioSource.Stop();
            isLowHealthActive = false;
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        AudioHelper.PlaySound(deathClip, audioSource);

        if (lowhealth_audioSource != null && lowhealth_audioSource.isPlaying)
            lowhealth_audioSource.Stop();

        OnDeath?.Invoke();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        HandleLowHealthAudio();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
