using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    [SerializeField] private PlayerController ghostController;
    [SerializeField] private PlayerHealthController ghostHealthController;

    private void Awake()
    {
        if (ghostController == null)
            Debug.LogError("UpgradeController requires a reference to the ghost PlayerController!");

        if (ghostHealthController == null)
            Debug.LogError("UpgradeController requires a reference to the ghost PlayerHealthController!");
    }

    public void IncreaseSpeed(float amount)
    {
        float newSpeed = ghostController.GetSpeed() + amount;
        ghostController.SetSpeed(newSpeed);
        Debug.Log($"Ghost speed increased to {newSpeed}");
    }

    public void IncreaseTurnSpeed(float amount)
    {
        float newTurnSpeed = ghostController.GetTurnSpeed() + amount;
        ghostController.SetTurnSpeed(newTurnSpeed);
        Debug.Log($"Ghost turn speed increased to {newTurnSpeed}");
    }

    public void IncreaseMaxHealth(float amount)
    {
        if (ghostController == null) return;

        float newMaxHealth = ghostController.GetMaxHealth() + amount;
        ghostController.SetMaxHealth(newMaxHealth);

        Debug.Log($"Ghost max health increased to {newMaxHealth}");
    }

    public void IncreaseAttackSpeed(float amount) 
    {
        float newSpeed = ghostController.GetAttackSpeed() + amount;
        ghostController.SetAttackSpeed(newSpeed);
        Debug.Log($"Ghost attack speed increased to {newSpeed}");
    }

    public void IncreaseSwipeDamage(float amount) 
    {
        float newDamage = ghostController.GetSwipeDamage() + amount;
        ghostController.SetSwipeDamage(newDamage);
        Debug.Log($"Ghost swipe damage increased to {newDamage}");
    }
    public void IncreaseScreamLevel(int amount)
    {
        int newScream = ghostController.GetScreamLevel() + amount;
        ghostController.SetScreamLevel(newScream);
        Debug.Log($"Scream level increased to {newScream}");
    }
    public void DecreasePossessCooldown(float amount) 
    {
        float newCooldown = ghostController.GetPossessCooldown() - amount;
        if (newCooldown < 0) newCooldown = 0;
        ghostController.SetPossessCooldown(newCooldown);
        Debug.Log($"Ghost possess cooldown decreased to {newCooldown}");
    }
    public void DecreaseScreamCooldown(float amount)
    {
        float newCooldown = ghostController.GetScreamCooldown() - amount;
        if (newCooldown < 0) newCooldown = 0;
        ghostController.SetScreamCooldown(newCooldown);
        Debug.Log($"Ghost Scream cooldown decreased to {newCooldown}");
    }
}

