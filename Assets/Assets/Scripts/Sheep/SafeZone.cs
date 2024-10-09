using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public int totalSheepCount;  // Total number of sheep that need to be captured
    private int capturedSheepCount = 0;  // How many sheep have been captured so far

    private BoxCollider safeZoneCollider;  // Reference to the Box Collider of the safe zone

    private void Start()
    {
        safeZoneCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        SheepMovement sheep = other.GetComponent<SheepMovement>();
        if (sheep != null && !sheep.IsInSafeZone())  // Only affect sheep that aren't already in the safe zone
        {
            // Stop the sheep from leaving the safe zone
            sheep.SetInSafeZone(true, safeZoneCollider.bounds);  // Pass the bounds of the safe zone

            // Increment captured sheep counter
            capturedSheepCount++;

            Debug.Log("Sheep Captured! Total Captured: " + capturedSheepCount);

            // Check if all sheep are captured
            if (capturedSheepCount >= totalSheepCount)
            {
                Debug.Log("All sheep are captured! You win!");
            }
        }
    }

    // Sheep should not leave, so no OnTriggerExit behavior
}
