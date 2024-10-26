using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    public GameObject wolfPrefab;                 // The wolf prefab to spawn
    public Collider spawnArea;                    // The area within which wolves should spawn
    public float minSpawnInterval = 5f;           // Minimum time interval between spawns
    public float maxSpawnInterval = 15f;          // Maximum time interval between spawns
    private int maxWolves = 2;                    // Maximum number of wolves allowed
    private int currentWolfCount = 0;

    [Header("UI Elements")]
    public Image wolfAlertImage;

    private void Start()
    {
        if (wolfAlertImage != null)
        {
            wolfAlertImage.enabled = false; // Hide the wolf alert image initially
        }
        StartCoroutine(SpawnWolvesContinuously());
    }

    private IEnumerator SpawnWolvesContinuously()
    {
        while (true)
        {
            // Check if we need more wolves
            if (currentWolfCount < maxWolves)
            {
                SpawnWolf();
            }
            else
            {
                // Toggle wolf alert image based on active wolf count
                wolfAlertImage.enabled = currentWolfCount > 0;
            }

            // Wait for a random interval before the next check
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SpawnWolf()
    {
        Vector3 spawnPosition = GetRandomPointInSpawnArea();
        GameObject wolf = Instantiate(wolfPrefab, spawnPosition, Quaternion.identity);
       // activeWolves.Add(wolf);
        currentWolfCount++;

        // Handle decrementing wolf count on destruction
        wolf.GetComponent<WolfBehavior>().OnWolfDeath += DecrementWolfCount;
    }

    private Vector3 GetRandomPointInSpawnArea()
    {
        Bounds bounds = spawnArea.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            bounds.min.y, // Keep on the ground level
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void DecrementWolfCount(GameObject wolf)
    {
       // if (activeWolves.Contains(wolf))
       // {
        //    activeWolves.Remove(wolf);
            currentWolfCount--;
      //  }

        // Update the UI image based on remaining wolves
        wolfAlertImage.enabled = currentWolfCount > 0;
    }
}
