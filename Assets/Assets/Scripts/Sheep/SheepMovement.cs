using System.Collections;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    // Enum to represent different states of the sheep
    private enum SheepState
    {
        Idle,       // Wandering or standing still
        Running     // Fleeing from the dog
    }

    private SheepState currentState = SheepState.Idle; // Initial state is Idle

    [Header("Wandering Settings")]
    public float minMoveDistance = 1f;   // Minimum wandering distance
    public float maxMoveDistance = 3f;   // Maximum wandering distance
    public float minMoveInterval = 1.5f; // Minimum time between random wandering
    public float maxMoveInterval = 4f;   // Maximum time between random wandering
    public float minSpeed = 1f;          // Minimum wandering speed
    public float maxSpeed = 3f;          // Maximum wandering speed

    [Header("Bark Response")]
    public float barkCooldown = 5f;      // Time sheep cannot wander after being barked at

    private Rigidbody rb;
    private float lastBarkedTime;        // Last time the sheep was barked at
    private bool isWandering = false;    // Whether the sheep is currently wandering

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(WanderCoroutine()); // Start wandering as soon as the game starts
    }

    private void Update()
    {
        // Rotate the sheep to face the movement direction if it’s moving
        if (rb.velocity.magnitude > 0.1f)
        {
            Vector3 direction = rb.velocity.normalized;
            direction.y = 0; // Keep the rotation only on the XZ plane
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    // Method to handle bark response
    public void OnBarkedAt(Vector3 barkForce)
    {
        if (currentState == SheepState.Running) return; // If already running, ignore further bark calls

        // Stop wandering if barked at
        StopCoroutine(WanderCoroutine());
        rb.velocity = barkForce; // Apply the bark force directly
        lastBarkedTime = Time.time; // Record the time the bark occurred
        currentState = SheepState.Running; // Set state to running
        isWandering = false; // Stop wandering
    }

    // Coroutine to manage wandering behavior
    private IEnumerator WanderCoroutine()
    {
        isWandering = true;

        while (true)
        {
            // Wait for the cooldown after a bark before wandering again
            if (Time.time - lastBarkedTime < barkCooldown)
            {
                currentState = SheepState.Idle;
                yield return null;
            }
            else
            {
                currentState = SheepState.Idle; // Set state back to idle once cooldown has passed

                // Choose random direction, distance, and speed
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                float randomDistance = Random.Range(minMoveDistance, maxMoveDistance);
                float randomSpeed = Random.Range(minSpeed, maxSpeed);

                // Move the sheep based on the calculated direction, distance, and speed
                Vector3 targetVelocity = randomDirection * randomSpeed;
                float moveTime = randomDistance / randomSpeed;

                float elapsedTime = 0f;
                while (elapsedTime < moveTime)
                {
                    if (currentState == SheepState.Running) yield break; // Stop wandering if barked at

                    rb.velocity = targetVelocity;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Stop the movement after the time has elapsed
                rb.velocity = Vector3.zero;

                // Wait for a random interval before the next movement
                float waitTime = Random.Range(minMoveInterval, maxMoveInterval);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
