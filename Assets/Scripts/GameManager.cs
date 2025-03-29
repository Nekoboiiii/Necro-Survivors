using System.Collections.Generic;
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
        GameOver,
        LevelUp
    }

    // Store the current state of the game
    public GameState currentState;
    // Store the previous state of the game
    public GameState prevouisState;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;


     [Header("Current Stats Displays")]
    // Curremt stat displays
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

     [Header("Results Screen")]
     public Image chosenCharacterImage; // Image of the chosen character
    public TextMeshProUGUI chosenCharacterName; // Name of the chosen character
    public TextMeshProUGUI levelReachedDisplay; // Level reached display
    public TextMeshProUGUI timeSurvivedDisplay; // Time survived display
    public List<Image> chosenWeaponsUI = new List<Image>(6); // List of Weapons UI
    public List<Image> chosenPassivItemsUI = new List<Image>(6); // List of passive items UI 

    [Header("Stopwatch")]
    public float timeLimit; // Time limit for the game
    float stopwatchTime; // Current time elapsed since the start of the stopwatch started
    public TextMeshProUGUI stopwatchDisplay; // Stopwatch display


    public bool isGameOver = false; // Flag to check if the game is over
    public bool ischoosingUpgrade = false; // Flag to check if the player is choosing an upgrade
    public GameObject PlayerObject; // Reference to the players game object

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
                UpdateStopwatch(); // Update the stopwatch
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
                    Time.timeScale = 0f; // Pause the game
                    Debug.Log("Game Over");
                    DisplayResults(); // Display the results
                }
                break;

            case GameState.LevelUp:
                // Handle level up logic here
                if(!ischoosingUpgrade)
                {
                    ischoosingUpgrade = true; // Set the flag to true when choosing an upgrade
                    Time.timeScale = 0f; // Pause the game
                    levelUpScreen.SetActive(true); // Show the level up screen
                    Debug.Log("Level Up Screen is Active");
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
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }
    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text; // Display the time survived
        ChangeState(GameState.GameOver); // Change the game state to GameOver
        Time.timeScale = 0f; // Pause the game
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true); // Show the results screen
    }

    public void AssignChosenCharacterUI(CharacterScriptableObject chosencharacterdata)
    {
        // Assign the chosen character's image and name to the UI elements
        chosenCharacterImage.sprite = chosencharacterdata.Icon;
        chosenCharacterName.text = chosencharacterdata.Name;
    }

    public void AssignLevelReachedUI(int levelReached)
    {
        // Assign the level reached to the UI element
        levelReachedDisplay.text = levelReached.ToString();
    }

    public void AssignChosenWeaponsAndPassivItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        // Assign the chosen weapons and passive items to the UI elements
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassivItemsUI.Count)
        {
            Debug.LogWarning("Weapons UI and Weapons Data do not match in size");
            return;
        }

        // Assign chosen weapons data to chosenWeaponsUI
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            // Check that the sprite of the corresponding UI element is not null
            if (chosenWeaponsData[i].sprite)
            {
                // Enable the corresponding UI element in chosenWeaponsUI and set its sprite to the sprite of the corresponding UI element in chosenWeaponsData
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                // Disable the UI element if no sprite is assigned
                chosenWeaponsUI[i].enabled = false; 
            }
        }

        for (int i = 0; i < chosenPassivItemsUI.Count; i++)
        {
            // Check that the sprite of the corresponding UI element is not null
            if (chosenPassiveItemsData[i].sprite)
            {
                // Enable the cprresponding UI element in chosenPassivItemsUI and set its sprite to the sprite of the corresponding UI element in chosenPassiveItemsData
                chosenPassivItemsUI[i].enabled = true;
                chosenPassivItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                // Disable the UI element if no sprite is assigned
                chosenPassivItemsUI[i].enabled = false; 
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime; // Increment the stopwatch time

        UpdateStopwatchDisplay(); // Update the stopwatch display

        if(stopwatchTime >= timeLimit)
        {
            GameOver(); // Trigger game over if the time limit is reached
        }
    }

    void UpdateStopwatchDisplay()
    {
        // Update the stopwatch display with the formatted time
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", (int)stopwatchTime / 60, (int)stopwatchTime % 60);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp); // Change the game state to LevelUp
        PlayerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
            ischoosingUpgrade = false; // Reset the flag
            Time.timeScale = 1f; // Reset the time limit
            levelUpScreen.SetActive(false); // Hide the level up screen
            ChangeState(GameState.Gameplay); // Change the game state to Gameplay
    }
}
