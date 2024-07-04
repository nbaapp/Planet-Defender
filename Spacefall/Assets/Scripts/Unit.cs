using UnityEngine;

public class Unit : MonoBehaviour
{
    protected Logic logic;
    public int maxHealth = 100; // Maximum health of the unit

    [SerializeField]
    protected int currentHealth; // Current health of the unit

    protected virtual void Start()
    {
        currentHealth = maxHealth; // Initialize health
        logic = FindObjectOfType<Logic>(); // Find the Logic script in the scene
    }

    public void TakeKnockback(Vector2 direction, float force)
    {
        // Apply a force in the specified direction
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Default death behavior: destroy the unit object
        Destroy(gameObject);
    }
}
