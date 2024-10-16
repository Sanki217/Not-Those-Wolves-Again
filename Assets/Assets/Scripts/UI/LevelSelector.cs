using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Material originalMaterial;      // The original material for the cubes
    [SerializeField] private Material selectedMaterial;      // The material applied when a cube is selected
    [SerializeField] private GameObject levelSelectionPopup; // Reference to the level selection confirmation popup
    [SerializeField] private TextMeshPro popupText;          // Reference to the TextMeshPro component for the popup text

    [SerializeField] private GameObject[] levelCubes;        // Array to hold references to level cubes (Level1Cube, Level2Cube, etc.)
    [SerializeField] private string[] levelNames;            // Array to hold the scene names (e.g., "Level1", "Level2")

    private GameObject selectedLevelCube;  // Reference to the selected level cube
    private string selectedLevelName;      // Stores the name of the selected level (e.g., "Level1", "Level2")
    private bool isPopupActive = false;    // Prevents further interaction when the popup is open

    private void Start()
    {
        levelSelectionPopup.SetActive(false); // Hide the confirmation popup at the start
        DisplayHighScores(); // Display the high scores at the start
    }

    private void Update()
    {
        // If the popup is active, prevent further interaction with level cubes
        if (isPopupActive) return;

        // Handle level cube clicks using Raycast
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object is a level cube
                if (hit.collider.CompareTag("LevelCube"))
                {
                    SelectLevelCube(hit.collider.gameObject);
                }
            }
        }
    }

    // Display high scores on each level cube (using world-space TextMeshPro)
    private void DisplayHighScores()
    {
        for (int i = 0; i < levelCubes.Length; i++)
        {
            GameObject levelCube = levelCubes[i];  // Get the current level cube
            string levelKey = levelNames[i] + "_HighScore";  // Create the key for the high score in PlayerPrefs
            int highScore = PlayerPrefs.GetInt(levelKey, 0); // Get the high score, default to 0 if none

            // Find the TextMeshPro component in the associated empty GameObject (on the level cube)
            Transform highScoreTextTransform = levelCube.transform.Find("HighScoreText");
            if (highScoreTextTransform != null)
            {
                TextMeshPro highScoreText = highScoreTextTransform.GetComponent<TextMeshPro>();
                if (highScoreText != null)
                {
                    highScoreText.text = "High Score: " + highScore;  // Display the high score on the 3D TextMeshPro
                }
                else
                {
                    Debug.LogError("TextMeshPro component not found on HighScoreText of " + levelCube.name);
                }
            }
            else
            {
                Debug.LogError("HighScoreText object not found on " + levelCube.name);
            }
        }
    }

    // Method to handle level cube selection
    private void SelectLevelCube(GameObject levelCube)
    {
        // If a level is already selected, revert its material to original
        if (selectedLevelCube != null)
        {
            selectedLevelCube.GetComponent<Renderer>().material = originalMaterial;
        }

        // Set the clicked cube as the selected level
        selectedLevelCube = levelCube;
        selectedLevelCube.GetComponent<Renderer>().material = selectedMaterial;

        // Set the selected level name based on the cube's name
        selectedLevelName = selectedLevelCube.name.Replace("Cube", ""); // e.g., "Level1", "Level2"

        // Find the empty GameObject associated with the cube (centered on the cube)
        Transform popupAnchor = selectedLevelCube.transform.Find("PopupAnchor");

        if (popupAnchor != null)
        {
            // Show the confirmation popup at the anchor's position
            levelSelectionPopup.SetActive(true);
            levelSelectionPopup.transform.position = Camera.main.WorldToScreenPoint(popupAnchor.position);
        }
        else
        {
            Debug.LogError("PopupAnchor not found on cube: " + selectedLevelCube.name);
        }

        // Update the popup text using TextMeshPro to reflect the selected level
        popupText.text = "Confirm " + selectedLevelName + "?";

        // Set popup as active to prevent further interaction
        isPopupActive = true;
    }

    // Method called when Yes is clicked in the popup
    public void ConfirmYes()
    {
        // Ensure the selected level name is correctly set to match the scene name
        SceneManager.LoadScene(selectedLevelName); // Load the corresponding scene (Level1, Level2, Level3)
    }

    // Method called when No is clicked in the popup
    public void ConfirmNo()
    {
        // Close the popup and revert the cube's material to original
        levelSelectionPopup.SetActive(false);
        if (selectedLevelCube != null)
        {
            selectedLevelCube.GetComponent<Renderer>().material = originalMaterial;
            selectedLevelCube = null; // Reset the selected level
        }

        // Enable interaction again
        isPopupActive = false;
    }

    // Method to go back to the Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // Load the Main Menu scene
    }
}
