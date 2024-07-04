using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    public GameObject walkerPrefab; // Prefab for the Walker alien
    public GameObject planet;
    public float spawnRadius = 30f; // Distance from the planet where aliens spawn
    public float spawnInterval = 5f; // Time between spawns
    private int maxAliensPerWave = 10; // Maximum number of aliens per wave

    private float nextSpawnTime;
    private int aliensSpawnedThisWave = 0; // Number of aliens spawned in the current wave

    void Update()
    {
        // Check if it's time to spawn a new alien and if the limit has not been reached
        if (Time.time >= nextSpawnTime && aliensSpawnedThisWave < maxAliensPerWave)
        {
            SpawnAlien();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    public void SetAliensPerWave(int num)
    {
        maxAliensPerWave = num;
    }

    void SpawnAlien()
    {
        // Calculate a random position around the planet
        Vector2 spawnPosition = (Vector2)planet.transform.position + Random.insideUnitCircle.normalized * spawnRadius;

        // Instantiate the Walker alien at the spawn position
        Instantiate(walkerPrefab, spawnPosition, Quaternion.identity);

        // Increment the counter for the number of aliens spawned this wave
        aliensSpawnedThisWave++;
    }

    // Call this method at the beginning of each new wave
    public void StartNewWave()
    {
        // Reset the counter for the number of aliens spawned
        aliensSpawnedThisWave = 0;
    }
}
