using System.Collections;
using UnityEngine;

public class FarmerBehavior : MonoBehaviour
{
    public int lives = 3;              // Farmer's lives
    public float wolfKillDelay = 2f;   // Delay before farmer kills a wolf after a bark
    public GameManager gameManager;    // Reference to GameManager

    private bool isKillingWolf = false; // Tracks if farmer is already killing a wolf

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    // Called when the dog barks at the farmer
    public void OnBarkFromDog()
    {
        // Check if there are active wolves
        if (gameManager.GetActiveWolfCount() > 0)
        {
            Debug.Log("Farmer hears bark and prepares to kill a wolf.");
            if (!isKillingWolf)
            {
                StartCoroutine(KillWolfAfterDelay());
            }
        }
        else
        {
            LoseLife();
        }
    }

    // Coroutine to kill a wolf after a delay
    private IEnumerator KillWolfAfterDelay()
    {
        isKillingWolf = true;
        yield return new WaitForSeconds(wolfKillDelay);

        if (gameManager.GetActiveWolfCount() > 0)
        {
            gameManager.KillRandomWolf();  // Let GameManager handle which wolf to kill
            Debug.Log("Farmer killed a wolf.");
        }
        isKillingWolf = false;
    }

    // Reduce farmer's life if there are no wolves
    private void LoseLife()
    {
        lives--;
        Debug.Log($"Farmer loses a life! Lives remaining: {lives}");

        if (lives <= 0)
        {
            Debug.Log("Farmer has no lives left. Farmer kills the player. Game over.");
            gameManager.GameOver();
        }
    }
}
