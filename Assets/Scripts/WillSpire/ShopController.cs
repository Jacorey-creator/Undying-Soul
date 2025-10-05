using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [SerializeField] private UpgradeController upgradeController;

    private int[] basePrices = { 10, 20, 25, 30, 50, 40 }; // Speed, Health, AttackSpeed, Damage, Scream, Possess
    private float[] priceMultipliers = { 1.2f, 1.3f, 1.35f, 1.4f, 1.5f, 1.45f };

    private int[] purchaseCounts; // tracks how many times each upgrade has been bought

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        purchaseCounts = new int[basePrices.Length];
    }

    public int GetUpgradePrice(int index)
    {
        return Mathf.CeilToInt(basePrices[index] * Mathf.Pow(priceMultipliers[index], purchaseCounts[index]));
    }

    public bool CanAffordUpgrade(int index, int playerEssence)
    {
        return playerEssence >= GetUpgradePrice(index);
    }

    public void BuyUpgrade(int index)
    {
        int price = GetUpgradePrice(index);
        var player = upgradeController.GetComponent<PlayerController>();

        if (player.GetEssenceStored() >= price)
        {
            player.SetEssenceStored(player.GetEssenceStored() - price);
            purchaseCounts[index]++;

            // Apply upgrade
            switch (index)
            {
                case 0: upgradeController.IncreaseSpeed(0.5f); break;
                case 1: upgradeController.IncreaseMaxHealth(2f); break;
                case 2: upgradeController.IncreaseAttackSpeed(0.05f); break;
                case 3: upgradeController.IncreaseSwipeDamage(0.5f); break;
                case 4:
                    upgradeController.IncreaseScreamLevel(1);
                    upgradeController.DecreaseScreamCooldown(0.25f);
                    break;
                case 5: upgradeController.DecreasePossessCooldown(1f); break;
            }

            Debug.Log($"Upgrade {index} purchased! New essence: {player.GetEssenceStored()}");
        }
        else
        {
            Debug.Log("Not enough essence!");
        }
    }

    //Added: Return the number of upgrades bought for a specific skill
    public int GetUpgradeLevel(int index)
    {
        if (index < 0 || index >= purchaseCounts.Length)
            return 0;

        return purchaseCounts[index];
    }
}
