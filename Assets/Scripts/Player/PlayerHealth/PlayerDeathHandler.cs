using UnityEngine;

[RequireComponent(typeof(PlayerHealthController))]
public class PlayerDeathHandler : MonoBehaviour
{
    private PlayerHealthController healthController;
    private PlayerController playerController;

    [Header("Death Settings")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private Canvas deathScreen;

    private void Awake()
    {
        healthController = GetComponent<PlayerHealthController>();
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        healthController.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        healthController.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Debug.Log("PlayerDeathHandler: Player has died.");

        // Disable player movement and input
        if (playerController != null)
            playerController.enabled = false;

        // Play death visual
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Show death screen (optional)
        if (deathScreen != null)
            deathScreen.enabled = true;

         Destroy(gameObject, 2f);
    }
}
