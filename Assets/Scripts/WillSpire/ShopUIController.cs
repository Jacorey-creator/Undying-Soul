using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopUIController : MonoBehaviour
{
    [Header("Panel & Animation")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(400, 0);
    [SerializeField] private Vector2 visiblePosition = Vector2.zero;

    [Header("Buttons & Prices")]
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedPriceText;
    [SerializeField] private Button healthButton;
    [SerializeField] private TextMeshProUGUI healthPriceText;
    [SerializeField] private Button attackSpeedButton;
    [SerializeField] private TextMeshProUGUI attackSpeedPriceText;
    [SerializeField] private Button swipeDamageButton;
    [SerializeField] private TextMeshProUGUI swipeDamagePriceText;
    [SerializeField] private Button screamButton;
    [SerializeField] private TextMeshProUGUI screamPriceText;
    [SerializeField] private Button possessButton;
    [SerializeField] private TextMeshProUGUI possessPriceText;

    [Header("Visual FeedBack")]
    [SerializeField] private SkillBar speedBar;
    [SerializeField] private SkillBar maxHealthBar;
    [SerializeField] private SkillBar attackSpeedBar;
    [SerializeField] private SkillBar swipeBar;
    [SerializeField] private SkillBar screamBar;
    [SerializeField] private SkillBar possessionBar;

    private bool isOpen = false;
    private ShopController shop;

    private void Awake()
    {
        // Ensure panel is active for RectTransform manipulation
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            panelRect.anchoredPosition = hiddenPosition;
            shopPanel.SetActive(false);
        }
        
    }

    private void Start()
    {
        shop = ShopController.Instance;
        if (shop == null)
        {
            Debug.LogError("ShopController instance not found in scene!");
        }
        // Assign button callbacks
        speedButton.onClick.AddListener(() => BuyUpgrade(0));
        healthButton.onClick.AddListener(() => BuyUpgrade(1));
        attackSpeedButton.onClick.AddListener(() => BuyUpgrade(2));
        swipeDamageButton.onClick.AddListener(() => BuyUpgrade(3));
        screamButton.onClick.AddListener(() => BuyUpgrade(4));
        possessButton.onClick.AddListener(() => BuyUpgrade(5));

        RefreshUI();
    }

    private void Update()
    {
        // Toggle shop via input (keyboard or joystick)
        if (Input.GetButtonDown("Shop"))
        {
            ToggleShop();
        }
    }

    private void ToggleShop()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        shopPanel.SetActive(true); // make sure panel is active to slide
        StartCoroutine(SlidePanel(isOpen ? visiblePosition : hiddenPosition));
        RefreshUI();
    }

    private IEnumerator SlidePanel(Vector2 target)
    {
        Vector2 start = panelRect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideDuration;
            panelRect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        panelRect.anchoredPosition = target;

        // Pause/unpause after slide finishes
        Time.timeScale = isOpen ? 0f : 1f;

        // Hide panel when fully closed
        if (!isOpen)
            shopPanel.SetActive(false);
    }

    private void BuyUpgrade(int index)
    {
        if (shop == null) return;

        shop.BuyUpgrade(index);
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (shop == null) return;

        int playerEssence = shop.GetComponent<PlayerController>().GetEssenceStored();

        UpdateButton(speedButton, speedPriceText, shop.GetUpgradePrice(0), playerEssence);
        UpdateButton(healthButton, healthPriceText, shop.GetUpgradePrice(1), playerEssence);
        UpdateButton(attackSpeedButton, attackSpeedPriceText, shop.GetUpgradePrice(2), playerEssence);
        UpdateButton(swipeDamageButton, swipeDamagePriceText, shop.GetUpgradePrice(3), playerEssence);
        UpdateButton(screamButton, screamPriceText, shop.GetUpgradePrice(4), playerEssence);
        UpdateButton(possessButton, possessPriceText, shop.GetUpgradePrice(5), playerEssence);

        // Update skill bars visually
        speedBar.SetLevel(shop.GetUpgradeLevel(0));
        maxHealthBar.SetLevel(shop.GetUpgradeLevel(1));
        attackSpeedBar.SetLevel(shop.GetUpgradeLevel(2));
        swipeBar.SetLevel(shop.GetUpgradeLevel(3));
        screamBar.SetLevel(shop.GetUpgradeLevel(4));
        possessionBar.SetLevel(shop.GetUpgradeLevel(5));
    }

    private void UpdateButton(Button button, TextMeshProUGUI priceText, int price, int playerEssence)
    {
        if (button == null || priceText == null) return;

        priceText.text = price.ToString();

        var color = (playerEssence < price) ? new Color(1f, 0.5f, 0.5f) : Color.white;
        var image = button.GetComponent<Image>();
        if (image != null) image.color = color;
    }
}
