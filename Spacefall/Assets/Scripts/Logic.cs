using UnityEngine;

public class Logic : MonoBehaviour
{
    public AlienSpawner alienSpawner; // Reference to the AlienSpawner
    [SerializeField] private int enemiesToSpawn;
    private int enemiesDefeated;
    private bool isBuildMode;
    public GameObject buildUI; // Reference to the build UI
    public GameObject attackUI; // Reference to the attack UI

    void Start()
    {
        // Start the first wave with the specified number of enemies
        StartNewWave(enemiesToSpawn);
    }

    // Method to start a new wave with a set number of enemies
    public void StartNewWave(int numberOfEnemies)
    {
        isBuildMode = false;
        enemiesDefeated = 0;
        alienSpawner.SetAliensPerWave(numberOfEnemies); // Set the number of enemies for the spawner
        alienSpawner.StartNewWave(); // Reset the spawner for the new wave
    }

    // Method to be called when an enemy is defeated
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= enemiesToSpawn)
        {
            EndWave();
        }
    }

    // Method to end the current wave
    private void EndWave()
    {
        Debug.Log("Wave ended!");
        EnterBuildMode();
        // Logic for ending the wave (e.g., give rewards, prepare for the next wave)
    }

    // Method to enter build mode
    private void EnterBuildMode()
    {
        isBuildMode = true;
        attackUI.SetActive(false); // Hide the attack UI
        buildUI.SetActive(true); // Show the build UI
        Debug.Log("Entered build mode!");
        // Logic for entering build mode (e.g., show build UI, enable building controls)
    }

    private void EnterAttackMode()
    {
        isBuildMode = false;

        buildUI.SetActive(false); // Hide the build UI
        attackUI.SetActive(true); // Show the attack UI

        Debug.Log("Entered attack mode!");
        // Logic for entering attack mode (e.g., show attack UI, enable attack controls)
    }

    // Method to start the next wave after build mode
    public void StartNextWave()
    {
        int nextWaveEnemies = enemiesToSpawn + 5; // Example: Increase the number of enemies for the next wave
        StartNewWave(nextWaveEnemies);
    }

    public bool IsBuildMode()
    {
        return isBuildMode;
    }
}
