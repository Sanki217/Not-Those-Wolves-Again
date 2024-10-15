using System.Collections;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{

    public int sheepIndex;

    private enum SheepState
    {
        Idle,       // Wandering or standing still
        Running     // Fleeing from the dog
    }

    private SheepState currentState = SheepState.Idle; // Initial state is Idle
    private bool isInSafeZone = false; // Whether the sheep is inside the safe zone
    private Bounds safeZoneBounds;  // Bounds of the safe zone

    [Header("Wandering Settings")]
    public float minMoveDistance = 1f;   // Minimum wandering distance
    public float maxMoveDistance = 3f;   // Maximum wandering distance
    public float minMoveInterval = 1.5f; // Minimum time between random wandering
    public float maxMoveInterval = 4f;   // Maximum time between random wandering
    public float minSpeed = 1f;          // Minimum wandering speed
    public float maxSpeed = 3f;          // Maximum wandering speed

    [Header("Bark Response")]
    public float barkCooldown = 5f;      // Time sheep cannot wander after being barked at
    public float minBarkForce = 1f;      // Minimum force that can be applied when barked at

    private Rigidbody rb;
    private float lastBarkedTime;        // Last time the sheep was barked at
    private bool isWandering = false;    // Whether the sheep is currently wandering

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Start wandering with a random initial delay
        float randomInitialDelay = Random.Range(0f, maxMoveInterval);
        StartCoroutine(StartWanderingAfterDelay(randomInitialDelay));
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
        if (isInSafeZone) return; // Ignore barking if the sheep is in the safe zone
        if (currentState == SheepState.Running) return; // If already running, ignore further bark calls

        // Stop wandering if barked at
        StopCoroutine(WanderCoroutine());
        rb.velocity = Vector3.zero; // Reset velocity before applying new force
        rb.AddForce(barkForce, ForceMode.Impulse); // Apply the bark force directly
        lastBarkedTime = Time.time; // Record the time the bark occurred
        currentState = SheepState.Running; // Set state to running
        isWandering = false; // Stop wandering

        // Ensure the force is always enough to move the sheep
        if (barkForce.magnitude < minBarkForce)
        {
            Vector3 adjustedForce = barkForce.normalized * minBarkForce;
            rb.AddForce(adjustedForce, ForceMode.Impulse);
        }

        // Restart the wandering after cooldown
        StartCoroutine(ResumeWanderingAfterCooldown());
    }

    // Method to set the sheep's safe zone status and bounds
    public void SetInSafeZone(bool inZone, Bounds bounds)
    {
        isInSafeZone = inZone;
        if (isInSafeZone)
        {
            safeZoneBounds = bounds;  // Store the bounds of the safe zone
        }
    }

    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }

    // Coroutine to start wandering after an initial random delay
    private IEnumerator StartWanderingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the random initial delay
        StartCoroutine(WanderCoroutine()); // Start the actual wandering behavior
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
                Vector3 targetPosition = transform.position + randomDirection * randomDistance;

                // Restrict movement to within the bounds of the safe zone if sheep is inside
                if (isInSafeZone)
                {
                    targetPosition = RestrictToBounds(targetPosition, safeZoneBounds);
                }

                Vector3 targetVelocity = (targetPosition - transform.position).normalized * randomSpeed;
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

    // Restrict the wandering position to stay within the bounds of the safe zone
    private Vector3 RestrictToBounds(Vector3 targetPosition, Bounds bounds)
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x),
            transform.position.y, // Keep the same Y position
            Mathf.Clamp(targetPosition.z, bounds.min.z, bounds.max.z)
        );
    }

    // Coroutine to resume wandering after the bark cooldown
    private IEnumerator ResumeWanderingAfterCooldown()
    {
        yield return new WaitForSeconds(barkCooldown);
        if (currentState == SheepState.Running && !isInSafeZone)
        {
            currentState = SheepState.Idle; // Reset to Idle state after cooldown
            StartCoroutine(WanderCoroutine()); // Resume wandering
        }
    }
}
