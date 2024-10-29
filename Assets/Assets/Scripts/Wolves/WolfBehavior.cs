using UnityEngine;
using System;

public class WolfBehavior : MonoBehaviour
{
    public float moveSpeed = 3f;                 // Speed at which the wolf follows the sheep
    public float detectionRadius = 20f;          // Maximum distance to detect sheep
    public event Action OnWolfDeath;             // Event to notify when a wolf dies

    private Transform targetSheep;
    private Rigidbody rb;
    private GameManager gameManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // Restrict rotation to Y-axis
        gameManager = FindObjectOfType<GameManager>();
        FindNearestSheep(); // Find the nearest sheep on spawn
    }

    private void Update()
    {
        if (targetSheep == null)
        {
            FindNearestSheep();
        }
        else
        {
            FollowSheep();
        }

        // Check if the wolf has fallen below the death threshold
        if (transform.position.y < gameManager.deathZone)
        {
            Destroy(gameObject);
            gameManager.WolfKilled();
        }
    }

    private void FindNearestSheep()
    {
        float closestDistance = detectionRadius;
        Transform closestSheep = null;

        foreach (SheepMovement sheep in FindObjectsOfType<SheepMovement>())
        {
            float distanceToSheep = Vector3.Distance(transform.position, sheep.transform.position);
            if (distanceToSheep < closestDistance)
            {
                closestDistance = distanceToSheep;
                closestSheep = sheep.transform;
            }
        }

        targetSheep = closestSheep;
    }

    private void FollowSheep()
    {
        if (targetSheep != null)
        {
            // Calculate the direction to the sheep only on the X and Z axes
            Vector3 directionToSheep = (targetSheep.position - transform.position).normalized;
            directionToSheep.y = 0; // Ignore Y-axis to prevent vertical movement

            // Calculate the movement vector based on the direction and speed
            Vector3 movement = directionToSheep * moveSpeed * Time.deltaTime;

            // Apply the movement only to X and Z, keeping Y unaffected by this movement
            rb.MovePosition(new Vector3(rb.position.x + movement.x, rb.position.y, rb.position.z + movement.z));

            // Rotate to face the sheep only on the Y-axis
            if (directionToSheep != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToSheep);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    public void KillWolf()
    {
        OnWolfDeath?.Invoke(); // Trigger the wolf death event
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            SheepMovement sheep = other.GetComponent<SheepMovement>();
            if (sheep != null)
            {
                gameManager.SheepDied(sheep.sheepIndex);  // Mark sheep as dead in GameManager
                Destroy(sheep.gameObject);                // Destroy the sheep
                FindNearestSheep();                       // Search for a new sheep target
            }
        }
        else if (other.CompareTag("Player"))
        {
            gameManager.GameOver(); // Trigger Game Over in GameManager
        }
    }
}
