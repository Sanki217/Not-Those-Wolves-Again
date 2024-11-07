using System.Collections;
using UnityEngine;

public class FarmerBehavior : MonoBehaviour
{
    public float shotDelay = 1f;  // Delay between each shot
    public GameObject shotVFX;  // VFX prefab for the shot
    public AudioSource shotAudio; // Audio source for shooting sound
    public Transform shootingPoint;  // Reference to the empty GameObject for VFX origin
    public Animator animator;     // Animator for farmer animations

    private GameManager gameManager;
    private WolfSpawner wolfSpawner;

    private void Start()
    {
        wolfSpawner = FindObjectOfType<WolfSpawner>();
        gameManager = FindObjectOfType<GameManager>();

        if (animator != null)
        {
            animator.Play("Idle"); // Set default animation to Idle
        }
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
            WolfBehavior[] wolves = FindObjectsOfType<WolfBehavior>();

            foreach (WolfBehavior wolf in wolves)
            {
                if (wolf != null)
                {
                    // Rotate the farmer towards the current wolf
                    RotateTowardWolf(wolf.transform.position);

                    // Play shoot animation once
                    if (animator != null)
                    {
                        animator.SetTrigger("ShootTrigger");

                    }

                    // Trigger VFX and sound at gun bone position
                    if (shotVFX != null && shootingPoint != null)
                    {
                        Instantiate(shotVFX, shootingPoint.position, shootingPoint.rotation);
                    }
                    if (shotAudio != null)
                    {
                        shotAudio.Play();
                    }

                    Debug.Log("Farmer shoots a wolf!");

                    // Destroy the wolf
                    wolf.KillWolf();
                    wolfSpawner.currentWolfCount -= 1;
                    gameManager.WolfKilled();

                    // Wait for the delay before the next shot
                    yield return new WaitForSeconds(shotDelay);
                }
            }

        }
    }

    private void RotateTowardWolf(Vector3 wolfPosition)
    {
        // Calculate direction to the wolf on the XZ plane
        Vector3 directionToWolf = (wolfPosition - transform.position).normalized;
        directionToWolf.y = 0;  // Ensure rotation only happens on the Y-axis

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToWolf);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10000f);  // Smooth rotation
    }

}
