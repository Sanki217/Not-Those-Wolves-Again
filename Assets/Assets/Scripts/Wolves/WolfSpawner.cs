using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    public GameObject wolfPrefab;                  // The wolf prefab to spawn
    public Collider spawnArea;                     // The area (Collider) within which wolves should spawn
    public float minSpawnInterval = 5f;            // Minimum time interval between spawns
    public float maxSpawnInterval = 15f;           // Maximum time interval between spawns
    public int maxWolves = 2;                      // Maximum number of wolves allowed at a time

    public int currentWolfCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnWolvesRandomly());
    }

    private IEnumerator SpawnWolvesRandomly()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            if (currentWolfCount < maxWolves)
            {
                SpawnWolf();
            }
        }
    }

    private void SpawnWolf()
    {
        Vector3 spawnPosition = GetRandomPointInSpawnArea();
        GameObject wolf = Instantiate(wolfPrefab, spawnPosition, Quaternion.identity);
        currentWolfCount++;

        // Attach event to decrement wolf count when a wolf dies
        WolfBehavior wolfBehavior = wolf.GetComponent<WolfBehavior>();
        if (wolfBehavior != null)
        {
            wolfBehavior.OnWolfDeath += DecrementWolfCount;
        }
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

    private void DecrementWolfCount()
    {
        currentWolfCount--;
    }
}
