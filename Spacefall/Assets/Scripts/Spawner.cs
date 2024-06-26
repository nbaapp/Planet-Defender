using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    public GameObject walkerPrefab; // Prefab for the Walker alien
    public GameObject planet;
    public float spawnRadius = 30f; // Distance from the planet where aliens spawn
    public float spawnInterval = 5f; // Time between spawns

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnAlien();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnAlien()
    {
        // Calculate a random position around the planet
        Vector2 spawnPosition = (Vector2)planet.transform.position + Random.insideUnitCircle.normalized * spawnRadius;

        // Instantiate the Walker alien at the spawn position
        Instantiate(walkerPrefab, spawnPosition, Quaternion.identity);
    }
}
