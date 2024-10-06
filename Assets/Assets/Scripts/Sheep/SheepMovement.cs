using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public float minMoveDistance = 1f;
    public float maxMoveDistance = 3f;
    public float moveSpeed = 2f;
    public float minMoveInterval = 1.5f; // in seconds
    public float maxMoveInterval = 4f;   // in seconds

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(MoveRandomlyCoroutine());
    }

    private IEnumerator MoveRandomlyCoroutine()
    {
        while (true)
        {
            // Generate a random direction
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // Generate a random move distance within the specified range
            float randomMoveDistance = Random.Range(minMoveDistance, maxMoveDistance);

            // Calculate the time it should take to reach the destination based on moveSpeed
            float moveTime = randomMoveDistance / moveSpeed;

            // Calculate the velocity needed to reach the destination in the specified time
            Vector2 velocity = randomDirection * moveSpeed;

            // Set the velocity for the Rigidbody2D
            rb.velocity = velocity;

            // Wait for the calculated move time before stopping the movement
            yield return new WaitForSeconds(moveTime);

            // Stop the sheep's movement by setting its velocity to zero
            rb.velocity = Vector2.zero;

            // Wait for a random interval in seconds before the next movement
            yield return new WaitForSeconds(Random.Range(minMoveInterval, maxMoveInterval));
        }
    }
}
