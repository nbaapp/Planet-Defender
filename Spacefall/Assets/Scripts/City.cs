using UnityEngine;

public class City : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the city
    private int currentHealth; // Current health of the city

    void Start()
    {
        // Initialize the city's health to the maximum value
        currentHealth = maxHealth;
    }

    // Method to apply damage to the city
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Check if the city's health has reached zero
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDestroyCity();
        }

        // Optionally, update the health UI or other visual indicators here
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

        // Ensure the health doesn't exceed the maximum
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Optionally, update the health UI or other visual indicators here
    }
}
