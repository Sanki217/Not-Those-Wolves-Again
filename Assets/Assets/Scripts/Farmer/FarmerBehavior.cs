using System.Collections;
using UnityEngine;

public class FarmerBehavior : MonoBehaviour
{
    public float shotDelay = 1f;  // Delay between each shot in seconds
    public GameObject shotVFX;    // Reference to the VFX prefab or effect for the shot
    public AudioSource shotAudio; // AudioSource for the shooting sound

    private GameManager gameManager;

    private WolfSpawner wolfSpawner;

    private void Start()
    {
        // Find the WolfSpawner in the scene to check the current wolf count
        wolfSpawner = FindObjectOfType<WolfSpawner>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnBarkedAt()
    {
        if (wolfSpawner != null && wolfSpawner.currentWolfCount > 0)
        {
            Debug.Log("Farmer hears the bark and starts shooting wolves!");
            StartCoroutine(ShootWolves());
        }
        else
        {
            Debug.Log("Farmer hears the bark but there are no wolves to shoot.");
        }
    }

    private IEnumerator ShootWolves()
    {
        while (wolfSpawner.currentWolfCount > 0)
        {
            // Get all active wolves in the scene
            WolfBehavior[] wolves = FindObjectsOfType<WolfBehavior>();

            foreach (WolfBehavior wolf in wolves)
            {
                if (wolf != null)
                {
                    // Trigger VFX and sound for each shot
                    if (shotVFX != null)
                    {
                        Instantiate(shotVFX, wolf.transform.position, Quaternion.identity);
                    }
                    if (shotAudio != null)
                    {
                        shotAudio.Play();
                    }

                    // Log shooting for debugging
                    Debug.Log("Farmer shoots a wolf!");

                    

                    // Destroy the wolf
                    wolf.KillWolf();
                    gameManager.WolfKilled();

                    // Wait for the delay before the next shot
                    yield return new WaitForSeconds(shotDelay);
                }
            }
        }
    }
}
