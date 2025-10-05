using UnityEngine;

public enum GameState { Gameplay, Shop }

public class GameStateManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; }

    public static void SetState(GameState newState)
    {
        CurrentState = newState;
        Time.timeScale = (newState == GameState.Gameplay) ? 1f : 0f;
    }

    public static bool IsGameplay => CurrentState == GameState.Gameplay;
}