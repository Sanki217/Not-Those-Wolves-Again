using System.Collections.Generic;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    public float barkRadius = 10f;          // Radius within which sheep and farmer react to the bark
    public LayerMask interactableLayer;     // Layer to detect sheep and farmer
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
        if (barkAudio != null)
        {
            barkAudio.Play();
            Debug.Log("Woof! The dog barked!");
        }

        // Use Physics.OverlapSphere to get all colliders in the bark radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, barkRadius, interactableLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Farmer"))
            {
                // Trigger farmer response if in range
             //   FarmerBehavior farmer = collider.GetComponent<FarmerBehavior>();
             //   if (farmer != null)
            //    {
            //        farmer.OnBarkFromDog();
             //   }
            }
            else if (collider.CompareTag("Sheep"))
            {
                // Handle sheep response to bark
                Rigidbody sheepRigidbody = collider.GetComponent<Rigidbody>();
                SheepMovement sheepMovement = collider.GetComponent<SheepMovement>();

                if (sheepRigidbody != null && sheepMovement != null)
                {
                    float distance = Vector3.Distance(dogTransform.position, collider.transform.position);

                    if (distance <= barkDistanceThreshold)
                    {
                        float distanceFactor = Mathf.Clamp01((barkDistanceThreshold - distance) / barkDistanceThreshold);
                        float barkForce = Mathf.Lerp(minBarkForce, maxBarkForce, distanceFactor);

                        Vector3 forceDirection = (collider.transform.position - dogTransform.position).normalized;
                        Vector3 barkForceVector = forceDirection * barkForce;
                        barkForceVector.y = verticalBarkForce;

                        sheepRigidbody.AddForce(barkForceVector, ForceMode.Impulse);
                        sheepMovement.OnBarkedAt(barkForceVector);
                    }
                }
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
