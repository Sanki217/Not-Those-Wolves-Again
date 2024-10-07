using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    public float barkRadius = 5f;        // Radius within which the sheep will react to the bark
    public LayerMask sheepLayer;         // Layer to detect sheep
    public float maxBarkForce = 10f;     // Maximum force applied to sheep when barking
    public float minBarkForce = 2f;      // Minimum force applied to sheep, even if they're far away

    private void Start()
    {
        // Ensure BarkController is working and attached to the dog
        if (!GetComponent<Rigidbody>())
        {
            Debug.LogError("BarkController should be attached to a GameObject with a Rigidbody!");
        }
    }

    public void Bark()
    {
        Debug.Log("Woof! The dog barked!");

        // Use Physics.OverlapSphere to get all colliders in the bark radius
        Collider[] sheepColliders = Physics.OverlapSphere(transform.position, barkRadius, sheepLayer);

        foreach (Collider collider in sheepColliders)
        {
            Rigidbody sheepRigidbody = collider.GetComponent<Rigidbody>();
            SheepMovement sheepMovement = collider.GetComponent<SheepMovement>();

            if (sheepRigidbody != null && sheepMovement != null)
            {
                // Calculate the distance between the dog and the sheep
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                // Calculate a force reduction factor based on distance (closer sheep get more force)
                float forceFactor = 1f - (distance / barkRadius);

                // Clamp the force factor to ensure it is within a controlled range
                forceFactor = Mathf.Clamp(forceFactor, 0.1f, 1f);

                // Calculate the bark force by mixing min and max forces
                float barkForce = Mathf.Lerp(minBarkForce, maxBarkForce, forceFactor);

                // Calculate the direction to push the sheep (away from the dog)
                Vector3 forceDirection = (collider.transform.position - transform.position).normalized;

                // Apply force only in the X-Z plane, so sheep don't fly upward
                forceDirection.y = 0;

                // Apply the calculated force to the sheep's Rigidbody
                sheepRigidbody.AddForce(forceDirection * barkForce, ForceMode.Impulse);

                // Notify the sheep of the bark so it interrupts wandering
                sheepMovement.OnBarkedAt(forceDirection * barkForce);
            }
        }
    }

    // Draw Gizmos to show the bark radius in the Editor for testing
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, barkRadius); // Visualize the bark radius
    }
}
