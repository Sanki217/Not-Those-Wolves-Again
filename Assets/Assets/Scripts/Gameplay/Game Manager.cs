using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] sheepIcons;  // Array of UI sheep icons
    [SerializeField] private Sprite sheepToCollectIcon;  // Icon for sheep to collect
    [SerializeField] private Sprite sheepCollectedIcon;  // Icon for sheep collected
    [SerializeField] private Sprite sheepDeadIcon;  // Icon for sheep dead
    [SerializeField] private GameObject gameOverPanel;  // Game Over UI Panel
    [SerializeField] private GameObject winPanel;  // Win UI Panel
    [SerializeField] private TextMeshProUGUI winPanelStats;  // Win panel stats

    [Header("Game Settings")]
    [SerializeField] private GameObject player;  // Reference to the player
    [SerializeField] private Transform[] sheep;  // Array of all sheep
    [SerializeField] public float deathZone =-5f;
    private bool[] sheepAlive;  // Track whether each sheep is alive or dead
    private int pointsToWin;  // Sheep needed to win
    private int sheepInSafeZone = 0;  // Sheep captured
    private int sheepDead = 0;  // Sheep that died
    private bool isGameOver = false;
    private bool isGameWon = false;

    [Header("Scoring")]
    private int startPoints = 1500;
    private int timePenaltyPerSecond = 50;
    private int pointsPerWolf = 100;
    private int wolvesKilled = 0;
    private float timeElapsed = 0f;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        Time.timeScale = 1f;

        pointsToWin = sheep.Length;  // Total sheep in the level

        sheepAlive = new bool[sheep.Length];  // Initialize array to track sheep alive status
        for (int i = 0; i < sheep.Length; i++)
        {
            sheepAlive[i] = true;  // Assume all sheep are alive at the start
        }

        InitializeSheepIcons();
    }

    private void Update()
    {
        if (!isGameOver && !isGameWon)
        {
            timeElapsed += Time.deltaTime;
            CheckFallConditions();
        }
    }

    // Initialize sheep icons for display
    private void InitializeSheepIcons()
    {
        for (int i = 0; i < sheep.Length; i++)
        {
            sheepIcons[i].GetComponent<UnityEngine.UI.Image>().sprite = sheepToCollectIcon;
        }
    }

    // Check if the player or sheep have fallen below Y = -2
    private void CheckFallConditions()
    {
        if (player.transform.position.y < deathZone)
        {
            GameOver();
        }

        for (int i = 0; i < sheep.Length; i++)
        {
            if (sheep[i] != null && sheep[i].position.y < deathZone && sheepAlive[i])
            {
                Destroy(sheep[i].gameObject);  // Sheep dies
                SheepDied(i);  // Call SheepDied when a sheep dies
            }
        }
    }

    // Check if a sheep is still alive
    public bool IsSheepAlive(int sheepIndex)
    {
        return sheepAlive[sheepIndex];  // Return true if the sheep is alive, false otherwise
    }

    // Called when a sheep enters the safe zone
    public void SheepInSafeZone(int sheepIndex)
    {
        if (!sheepAlive[sheepIndex])
            return;  // Ignore if the sheep is already dead or in the safe zone

        sheepInSafeZone++;
        pointsToWin--;

        // Mark the sheep as no longer alive since it's in the safe zone
        sheepAlive[sheepIndex] = false;

        // Update the sheep icon to "Sheep Collected"
        sheepIcons[sheepIndex].GetComponent<UnityEngine.UI.Image>().sprite = sheepCollectedIcon;

        // Check if all sheep (collected or dead) are accounted for
        if (sheepInSafeZone + sheepDead >= sheep.Length)
        {
            WinGame();
        }

        Debug.Log("Sheep in Safe Zone: " + sheepInSafeZone);
    }

    // Called when a sheep dies
    public void SheepDied(int sheepIndex)
    {
        sheepDead++;
        pointsToWin--;  // Decrease the number of sheep required to win

        // Mark the sheep as no longer alive
        sheepAlive[sheepIndex] = false;

        // Update the sheep icon to "Sheep Dead"
        sheepIcons[sheepIndex].GetComponent<UnityEngine.UI.Image>().sprite = sheepDeadIcon;

        // Check if all sheep (collected or dead) are accounted for
        if (sheepInSafeZone + sheepDead >= sheep.Length)
        {
            WinGame();
        }

        Debug.Log("Sheep died! Total Dead: " + sheepDead);
    }

    // Handle Game Over logic
    public void GameOver()
    {
        Debug.Log("Game Over! Player has died.");
        isGameOver = true;

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Handle Win condition
    private void WinGame()
    {
        isGameWon = true;
        Time.timeScale = 0f;
        Debug.Log("You Win! All sheep collected or accounted for.");

        // Calculate final score
        int remainingPoints = Mathf.Max(0, startPoints - (int)(timeElapsed * timePenaltyPerSecond));
        int finalScore = (remainingPoints + wolvesKilled * pointsPerWolf) * sheepInSafeZone;

        winPanelStats.text = $"Time: {timeElapsed:F2} sec\n" +
                             $"Sheep Saved: {sheepInSafeZone} / {sheep.Length}\n" +
                             $"Wolves Killed: {wolvesKilled}\n" +
                             $"Final Score: {finalScore}";
        winPanel.SetActive(true);

        SaveHighScore(finalScore);
    }

    // Save High Score
    private void SaveHighScore(int finalScore)
    {
        string levelKey = SceneManager.GetActiveScene().name + "_HighScore";
        int currentHighScore = PlayerPrefs.GetInt(levelKey, 0);
        if (finalScore > currentHighScore)
        {
            PlayerPrefs.SetInt(levelKey, finalScore);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + finalScore);
        }
        else
        {
            Debug.Log("High Score remains: " + currentHighScore);
        }
    }

    // Restart the current level
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Go to Level Selector
    public void GoToLevelSelector()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelectorScene");
    }

    // Go to Main Menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    // Method to track wolf kills
    public void WolfKilled()
    {
        wolvesKilled++;
        Debug.Log("Wolf killed! Total wolves killed: " + wolvesKilled);
    }
}