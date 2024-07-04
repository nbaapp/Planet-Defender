using UnityEngine;

public class City : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the city
    [SerializeField]
    private int currentHealth; // Current health of the city
    public HealthBar healthBar; // Reference to the HealthBar script

    void Start()
    {
        // Initialize the city's health to the maximum value
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth); // Initialize the health bar
    }

    // Method to apply damage to the city
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth); // Update the health bar

        // Check if the city's health has reached zero
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDestroyCity();
        }
    }

    // Method called when the city's health reaches zero
    void OnDestroyCity()
    {
        // Perform actions needed when the city is destroyed (e.g., game over, play animation)
        Debug.Log("City destroyed!");

        // Destroy the city object (optional, depending on game design)
        Destroy(gameObject);
    }

    // Method to heal the city (optional, if needed in gameplay)
    public void Heal(int amount)
    {
        currentHealth += amount;
        healthBar.SetHealth(currentHealth); // Update the health bar

        // Ensure the health doesn't exceed the maximum
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
