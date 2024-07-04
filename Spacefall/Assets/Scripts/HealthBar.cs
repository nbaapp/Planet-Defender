using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Reference to the slider UI component

    // Method to initialize the health bar
    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    // Method to update the current health value
    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }
}
