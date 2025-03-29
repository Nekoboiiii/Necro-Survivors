using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance
    // Define the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    // Store the current state of the game
    public GameState currentState;
    // Store the previous state of the game
    public GameState prevouisState;

    [Header("UI")]
    public GameObject pauseScreen;

    // Curremt stat displays
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    // Flag to check if the game is over
    public bool isGameOver = false; // Flag to check if the game is over

    void Awake()
    {
        // Warning check to see if there is antother singeton of thos kind in the game
        if (instance == null)
        {
            instance = this; // Set the singleton instance
        }
        else if (instance != this)
        {
            Debug.LogWarning("Extra" + this + "Deleted");
        }
        
        DiscableScreens(); // Disable all screens at the start
    }

    void Update()
    {
        // Define the behavior for each game state

        switch (currentState)
        {
            case GameState.Gameplay:
                // Handle gameplay logic here
                CheckForPauseAndResume();
                break;

            case GameState.Paused:
                // Handle pause logic here
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                // Handle game over logic here
                if(!isGameOver)
                {
                    isGameOver = true;
                    Debug.Log("Game Over");
                    DisplayResults(); // Display the results
                }
                break;

                default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        // Change the current state of the game
        currentState = newState;
        Debug.Log("Game State Changed to: " + currentState);
    }

    public void PauseGame()
    {
        if(currentState != GameState.Paused)
        {
            prevouisState = currentState; // Store the previous state
            // Pause the game and set the state to Paused
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true); // Show the pause screen
            Debug.Log("Game is Paused");
        }
        
        
    }
    public void ResumeGame()
    {
       if(currentState == GameState.Paused)
        {
            ChangeState(prevouisState);// Restore the previous state
            // Resume the game and set the state to Gameplay
            Time.timeScale = 1f;
            pauseScreen.SetActive(false); // Hide the pause screen
            Debug.Log("Game is Resumed");
        }
        
    }

    void CheckForPauseAndResume()
    {
        // Check for pause and resume input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DiscableScreens()
    {
        // Disable all screens
        pauseScreen.SetActive(false);
    }
    public void GameOver()
    {
        ChangeState(GameState.GameOver); // Change the game state to GameOver
        Time.timeScale = 0f; // Pause the game
    }

    void DisplayResults()
    {

    }

}
