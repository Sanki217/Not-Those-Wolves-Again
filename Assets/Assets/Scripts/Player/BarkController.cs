using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    public float barkRadius = 5f;
    public LayerMask sheepLayer;
    public float maxBarkForce = 10f;

    public void Bark()
    {
        Collider2D[] sheepColliders = Physics2D.OverlapCircleAll(transform.position, barkRadius, sheepLayer);
        foreach (Collider2D collider in sheepColliders)
        {
            Rigidbody2D sheepRigidbody = collider.GetComponent<Rigidbody2D>();
            if (sheepRigidbody != null)
            {
                // Calculate the distance between the dog and the sheep
                float distance = Vector2.Distance(transform.position, collider.transform.position);

                // Calculate a force reduction factor based on distance
                float forceFactor = 1f - (distance / barkRadius);

                // Apply reduced force to the sheep
                float barkForce = maxBarkForce * forceFactor;
                Vector2 forceDirection = (collider.transform.position - transform.position).normalized;
                sheepRigidbody.AddForce(forceDirection * barkForce, ForceMode2D.Impulse);
            }
        }

        Debug.Log("Woof! The dog barked!");
    }
}
