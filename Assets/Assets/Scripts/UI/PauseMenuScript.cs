using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public Button resumeButton;
    public Button levelSelectorButton;
    public Button mainMenuButton;
    public Text promptText;
    public Button yesButton;
    public Button noButton;
    public GameObject PauseMenuPanel;

    private bool isPaused = false;
    private bool isPromptVisible = false;

    void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        levelSelectorButton.onClick.AddListener(ShowLevelSelectorPrompt);
        mainMenuButton.onClick.AddListener(ShowMainMenuPrompt);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        PauseMenuPanel.SetActive(true);
    }

    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        PauseMenuPanel.SetActive(false);
    }

    void ShowLevelSelectorPrompt()
    {
        isPromptVisible = true;
        promptText.text = "Would you like to proceed?";
        yesButton.onClick.AddListener(LoadLevelSelectorScene);
        noButton.onClick.AddListener(HidePrompt);
    }

    void ShowMainMenuPrompt()
    {
        isPromptVisible = true;
        promptText.text = "Would you like to proceed?";
        yesButton.onClick.AddListener(LoadMainMenuScene);
        noButton.onClick.AddListener(HidePrompt);
    }

    void LoadLevelSelectorScene()
    {
        isPromptVisible = false;
        // Load the level selector scene here
        // For example:
        SceneManager.LoadScene("LevelSelectorScene");
    }

    void LoadMainMenuScene()
    {
        isPromptVisible = false;
        // Load the main menu scene here
        // For example:
        SceneManager.LoadScene("MainMenuScene");
    }

    void HidePrompt()
    {
        isPromptVisible = false;
    }
}