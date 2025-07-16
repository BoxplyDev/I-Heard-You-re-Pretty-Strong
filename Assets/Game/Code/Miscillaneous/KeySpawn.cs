using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KeySpawn : MonoBehaviour
{
    [Header("Key Spawn Points")]
    public Transform[] spawnPoints;
    public GameObject keyPrefab;
    public int numberOfKeys = 3;

    void Start()
    {
        SpawnKeys();
    }

    void SpawnKeys()
    {
        // Check if there are enough spawn points
        if (spawnPoints.Length < numberOfKeys)
        {
            Debug.LogError("Not enough spawn points to spawn the keys!");
            return;
        }

        // Create a list of available spawn points
        List<int> availableSpawnPoints = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        // Randomly select unique spawn points
        for (int i = 0; i < numberOfKeys; i++)
        {
            // Select a random index from the available spawn points
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomIndex];

            // Instantiate the key prefab at the chosen spawn point
            Instantiate(keyPrefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);

            // Remove the chosen spawn point from the list to ensure uniqueness
            availableSpawnPoints.RemoveAt(randomIndex);
        }
    }
}
