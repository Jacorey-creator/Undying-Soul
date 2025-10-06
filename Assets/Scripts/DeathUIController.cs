using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject deathPanel; // Your death UI panel
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Main menu scene name

    private bool isDead = false;

    private void Start()
    {
        deathPanel.SetActive(false);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    // Call this when the player dies
    public void ShowDeathScreen()
    {
        if (isDead) return;
        isDead = true;

        deathPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
