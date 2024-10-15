using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private BoxCollider safeZoneCollider;  // Reference to the Box Collider of the safe zone

    [SerializeField] private GameManager gameManager;  // Reference to the GameManager

    private void Start()
    {
        // Initialize the BoxCollider of the safe zone
        safeZoneCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is a sheep and it's not already in the safe zone
        SheepMovement sheep = other.GetComponent<SheepMovement>();
        if (sheep != null && !sheep.IsInSafeZone())
        {
            // Prevent the sheep from leaving the safe zone by marking it as captured
            sheep.SetInSafeZone(true, safeZoneCollider.bounds);  // Pass the bounds of the safe zone

            // Check if the sheep has not been destroyed (dead) and then notify the GameManager
            if (gameManager.IsSheepAlive(sheep.sheepIndex))
            {
                gameManager.SheepInSafeZone(sheep.sheepIndex);  // Only call if the sheep is alive
            }
        }
    }
}
