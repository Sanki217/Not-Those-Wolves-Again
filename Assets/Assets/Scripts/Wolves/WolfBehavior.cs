using UnityEngine;
using System;

public class WolfBehavior : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float detectionRadius = 20f;

    public event Action<GameObject> OnWolfDeath; // Event to notify when a wolf dies

    private Transform targetSheep;
    private Rigidbody rb;
    private GameManager gameManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        FindNearestSheep();
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
    }

    private void FindNearestSheep()
    {
        float closestDistance = detectionRadius;
        Transform closestSheep = null;

        foreach (Transform sheepTransform in gameManager.GetActiveSheep())
        {
            float distanceToSheep = Vector3.Distance(transform.position, sheepTransform.position);
            if (distanceToSheep < closestDistance)
            {
                closestDistance = distanceToSheep;
                closestSheep = sheepTransform;
            }
        }

        targetSheep = closestSheep;
    }

    private void FollowSheep()
    {
        if (targetSheep != null)
        {
            Vector3 directionToSheep = (targetSheep.position - transform.position).normalized;
            Vector3 movement = new Vector3(directionToSheep.x, 0, directionToSheep.z) * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);

            if (directionToSheep != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToSheep.x, 0, directionToSheep.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            SheepMovement sheep = other.GetComponent<SheepMovement>();
            if (sheep != null)
            {
                gameManager.SheepDied(sheep.sheepIndex);
                Destroy(sheep.gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            gameManager.GameOver();
        }
    }

    public void KillWolf()
    {
        
        Destroy(gameObject);
    }
}
