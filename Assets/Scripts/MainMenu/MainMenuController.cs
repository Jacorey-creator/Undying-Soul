using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("The name of the scene to load when starting the game.")]
    public string gameSceneName = "GameScene";

    /// <summary>
    /// Called when the Start Game button is clicked.
    /// </summary>
    public void StartGame()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogWarning("Game scene name is empty. Assign it in the inspector!");
        }
    }

    /// <summary>
    /// Called when the Quit button is clicked.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
        Application.Quit(); // Quit in build
#endif
    }
}
