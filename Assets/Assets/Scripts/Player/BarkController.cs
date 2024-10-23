using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    public float barkRadius = 10f;          // Radius within which the sheep will react to the bark
    public LayerMask sheepLayer;            // Layer to detect sheep
    public float maxBarkForce = 7f;         // Maximum force applied to sheep when barking
    public float minBarkForce = 1f;         // Minimum force applied to sheep, even if they're far away
    public float barkDistanceThreshold = 10f;  // Distance beyond which sheep are unaffected
    public float verticalBarkForce = 2f;    // Vertical force applied to the sheep when barking
    public AudioSource barkAudio;

    // Reference to the Dog's position
    private Transform dogTransform;

    private void Start()
    {
        dogTransform = transform; // Dog's transform is this object's transform
    }

    public void Bark()
    {
        Debug.Log("Woof! The dog barked!");

        if (barkAudio != null)
        {
            barkAudio.Play();
        }

        // Use Physics.OverlapSphere to get all colliders in the bark radius
        Collider[] sheepColliders = Physics.OverlapSphere(transform.position, barkRadius, sheepLayer);

        foreach (Collider collider in sheepColliders)
        {
            Rigidbody sheepRigidbody = collider.GetComponent<Rigidbody>();
            SheepMovement sheepMovement = collider.GetComponent<SheepMovement>();

            if (sheepRigidbody != null && sheepMovement != null)
            {
                // Calculate the distance between the dog and the sheep
                float distance = Vector3.Distance(dogTransform.position, collider.transform.position);

                // If the sheep is farther than the threshold, ignore the barking
                if (distance > barkDistanceThreshold)
                {
                    continue;  // Skip this sheep since it's too far away to be affected
                }

                // Calculate the force reduction factor based on the distance (closer sheep get more force)
                float distanceFactor = Mathf.Clamp01((barkDistanceThreshold - distance) / barkDistanceThreshold); // Maps distance to a 0-1 range
                float barkForce = Mathf.Lerp(minBarkForce, maxBarkForce, distanceFactor);

                // Calculate the direction to push the sheep (away from the dog)
                Vector3 forceDirection = (collider.transform.position - dogTransform.position).normalized;

                // Apply both horizontal and vertical force
                Vector3 barkForceVector = forceDirection * barkForce;
                barkForceVector.y = verticalBarkForce;  // Add vertical component to launch sheep into the air

                // Apply the calculated force to the sheep's Rigidbody
                sheepRigidbody.AddForce(barkForceVector, ForceMode.Impulse);

                // Notify the sheep of the bark so it interrupts wandering
                sheepMovement.OnBarkedAt(barkForceVector);
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
