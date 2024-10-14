using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro namespace

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Material originalMaterial;      // The original material for the cubes
    [SerializeField] private Material selectedMaterial;      // The material applied when a cube is selected
    [SerializeField] private GameObject levelSelectionPopup; // Reference to the level selection confirmation popup
    [SerializeField] private TextMeshProUGUI popupText;      // Reference to the TextMeshPro component for the popup text

    private GameObject selectedLevelCube;  // Reference to the selected level cube
    private string selectedLevelName;      // Stores the name of the selected level (e.g., "Level1", "Level2")
    private bool isPopupActive = false;    // Prevents further interaction when the popup is open

    private void Start()
    {
        levelSelectionPopup.SetActive(false); // Hide the confirmation popup at the start
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
        // Assume cubes are named "Level1Cube", "Level2Cube", etc.
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
