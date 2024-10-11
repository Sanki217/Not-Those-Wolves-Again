using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;      // Reference to the Pause Menu Panel
    [SerializeField] private GameObject confirmationPopup;   // Reference to the Confirmation Popup Panel
    private bool isPaused = false;                           // Keeps track of whether the game is paused
    private string sceneToLoad;                              // Stores which scene to load (Level Selector or Main Menu)

    private void Start()
    {
        // Ensure the pause menu and confirmation popup are hidden at the start
        pauseMenuPanel.SetActive(false);
        confirmationPopup.SetActive(false);
    }

    // Method to handle pause button click
    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true); // Show the pause menu
        Time.timeScale = 0f;            // Stop the game by setting timescale to 0
    }

    // Method to handle resume button click
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f;             // Resume the game by setting timescale back to 1
    }

    // Method to show confirmation popup for Level Selector
    public void ConfirmLevelSelectorScene()
    {
        sceneToLoad = "LevelSelectorScene";  // Store the scene name for Level Selector
        ShowConfirmationPopup();
    }

    // Method to show confirmation popup for Main Menu
    public void ConfirmMainMenuScene()
    {
        sceneToLoad = "MainMenuScene";  // Store the scene name for Main Menu
        ShowConfirmationPopup();
    }

    // Method to show the confirmation popup
    private void ShowConfirmationPopup()
    {
        pauseMenuPanel.SetActive(false);    // Hide the pause menu
        confirmationPopup.SetActive(true);  // Show the confirmation popup
    }

    // Method to handle Yes button in confirmation popup
    public void ConfirmYes()
    {
        Time.timeScale = 1f; // Reset timescale before changing scenes
        SceneManager.LoadScene(sceneToLoad);  // Load the stored scene (Level Selector or Main Menu)
    }

    // Method to handle No button in confirmation popup
    public void ConfirmNo()
    {
        confirmationPopup.SetActive(false);  // Close the confirmation popup
        pauseMenuPanel.SetActive(true);      // Return to the pause menu
    }
}
