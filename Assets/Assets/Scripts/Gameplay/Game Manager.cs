using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject[] sheepIcons;
    [SerializeField] private Sprite sheepToCollectIcon;
    [SerializeField] private Sprite sheepCollectedIcon;
    [SerializeField] private Sprite sheepDeadIcon;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI winPanelStats;

    [Header("Game Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] sheep;
    [SerializeField] public float deathZone = -5f;
    private bool[] sheepAlive;
    private int pointsToWin;
    private int sheepInSafeZone = 0;
    private int sheepDead = 0;
    private bool isGameOver = false;
    private bool isGameWon = false;

    [Header("Scoring")]
    private int startPoints = 1500;
    private int timePenaltyPerSecond = 50;
    private int pointsPerWolf = 100;
    private int wolvesKilled = 0;
    private float timeElapsed = 0f;

    [Header("Sound Effects")]
    public AudioSource dogDeathSound;  // Zmienna dla dŸwiêku œmierci psa

    private void Start()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        Time.timeScale = 1f;

        pointsToWin = sheep.Length;

        sheepAlive = new bool[sheep.Length];
        for (int i = 0; i < sheep.Length; i++)
        {
            sheepAlive[i] = true;
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

    private void InitializeSheepIcons()
    {
        for (int i = 0; i < sheep.Length; i++)
        {
            sheepIcons[i].GetComponent<UnityEngine.UI.Image>().sprite = sheepToCollectIcon;
        }
    }

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
                Destroy(sheep[i].gameObject);
                SheepDied(i);
            }
        }
    }

    public bool IsSheepAlive(int sheepIndex)
    {
        return sheepAlive[sheepIndex];
    }

    public void SheepInSafeZone(int sheepIndex)
    {
        if (!sheepAlive[sheepIndex])
            return;

        sheepInSafeZone++;
        pointsToWin--;

        sheepAlive[sheepIndex] = false;

        sheepIcons[sheepIndex].GetComponent<UnityEngine.UI.Image>().sprite = sheepCollectedIcon;

        if (sheepInSafeZone + sheepDead >= sheep.Length)
        {
            WinGame();
        }

        Debug.Log("Sheep in Safe Zone: " + sheepInSafeZone);
    }

    public void SheepDied(int sheepIndex)
    {
        sheepDead++;
        pointsToWin--;

        sheepAlive[sheepIndex] = false;

        sheepIcons[sheepIndex].GetComponent<UnityEngine.UI.Image>().sprite = sheepDeadIcon;

        if (sheepInSafeZone + sheepDead >= sheep.Length)
        {
            WinGame();
        }

        Debug.Log("Sheep died! Total Dead: " + sheepDead);
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Player has died.");
        isGameOver = true;

        if (dogDeathSound != null)
        {
            dogDeathSound.Play();  // Odtwórz dŸwiêk œmierci psa
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void WinGame()
    {
        isGameWon = true;
        Time.timeScale = 0f;
        Debug.Log("You Win! All sheep collected or accounted for.");

        int remainingPoints = Mathf.Max(0, startPoints - (int)(timeElapsed * timePenaltyPerSecond));
        int finalScore = (remainingPoints + wolvesKilled * pointsPerWolf) * sheepInSafeZone;

        winPanelStats.text = $"Time: {timeElapsed:F2} sec\n" +
                             $"Sheep Saved: {sheepInSafeZone} / {sheep.Length}\n" +
                             $"Wolves Killed: {wolvesKilled}\n" +
                             $"Final Score: {finalScore}";
        winPanel.SetActive(true);

        SaveHighScore(finalScore);
    }

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

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLevelSelector()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelectorScene");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void WolfKilled()
    {
        wolvesKilled++;
        Debug.Log("Wolf killed! Total wolves killed: " + wolvesKilled);
    }
}