using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject soundOnIcon;     // Sound ON icon (active when sound is on)
    [SerializeField] private GameObject soundOffIcon;    // Sound OFF icon (active when sound is muted)
    [SerializeField] private GameObject howToPlayPanel;  // The panel that contains the HowToPlay tutorial overlay
    private bool isMuted = false;                        // Boolean to track whether the sound is muted

    private void Start()
    {
        // Ensure sound is ON by default
        AudioListener.pause = false;
        isMuted = false;
        UpdateSoundIcon();

        // Hide the HowToPlay panel at the start
        howToPlayPanel.SetActive(false);
    }

    // Method for Play button to load the Level Selector scene
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelectorScene");  // Replace with your Level Selector scene name
    }

    // Method to show the HowToPlay panel
    public void ShowHowToPlay()
    {
        howToPlayPanel.SetActive(true); // Show the tutorial overlay
    }

    // Method to hide the HowToPlay panel (called when X is clicked)
    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false); // Hide the tutorial overlay
    }

    // Method to handle Exit button (quits the game)
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");  // This will not show in a build, but it helps when testing in the editor
    }

    // Method to toggle sound ON and OFF
    public void ToggleSound()
    {
        isMuted = !isMuted;   // Toggle the sound state
        AudioListener.pause = isMuted;   // Mute or unmute the global audio listener
        UpdateSoundIcon();    // Update the sound button icon
    }

    // Method to update the sound button's icon
    private void UpdateSoundIcon()
    {
        if (isMuted)
        {
            soundOnIcon.SetActive(false);
            soundOffIcon.SetActive(true);
        }
        else
        {
            soundOnIcon.SetActive(true);
            soundOffIcon.SetActive(false);
        }
    }
}
