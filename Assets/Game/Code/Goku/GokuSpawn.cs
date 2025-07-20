using UnityEngine;

public class GokuSpawn : MonoBehaviour
{
    public GameObject goku; // Assign Goku prefab or instance in the Inspector

    private void Awake()
    {
        if (goku == null)
        {
            Debug.LogError("Goku not assigned to GokuSpawn!");
            return;
        }

        // Get all child transforms (excluding self)
        Transform[] spawnPoints = GetComponentsInChildren<Transform>();
        if (spawnPoints.Length <= 1)
        {
            Debug.LogError("No spawn points set under GokuSpawn object.");
            return;
        }

        // Exclude the parent transform (this transform)
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform chosenSpawn = spawnPoints[randomIndex];

        // Move Goku to the spawn point before anything else
        goku.transform.position = chosenSpawn.position;
    }
}