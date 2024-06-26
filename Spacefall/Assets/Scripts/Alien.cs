using UnityEngine;

public class Alien : MonoBehaviour
{
    public int maxHealth = 50; // Maximum health of the alien
    protected int currentHealth; // Current health of the alien
    public float moveSpeed = 2f; // Movement speed of the alien

    protected Transform targetCity; // The city that the alien is targeting
    protected bool isGrounded; // Check if the alien is grounded
    protected Transform planetCenter;

    protected virtual void Start()
    {
        currentHealth = maxHealth; // Initialize health
        planetCenter = GameObject.FindWithTag("Planet").transform;
    }

    protected virtual void FixedUpdate()
    {
        AlignToPlanetSurface();
    }

    public void TakeKnockback(Vector2 direction, float force)
    {
        // Apply a force in the specified direction
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    // Method to apply damage to the alien
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to be overridden by specific alien types for custom death behavior
    protected virtual void Die()
    {
        // Destroy the alien object (can be overridden for custom behavior)
        Destroy(gameObject);
    }

    // Method to find the nearest city (could be implemented to get the closest target city)
    protected void FindNearestCity()
    {
        // Implement logic to find and set the nearest city to targetCity

        City[] cities = FindObjectsOfType<City>();
        if (cities.Length == 0) return;

        float closestDistance = float.MaxValue;
        foreach (var city in cities)
        {
            float distance = Vector2.Distance(transform.position, city.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetCity = city.transform;
            }
        }
    }

    protected void MoveAroundPlanet(Vector2 direction)
    {
        if (planetCenter == null) return;

        // Move around the planet in the specified direction
        Vector3 relativePosition = (planetCenter.position - transform.position).normalized;
        float angle = Mathf.Atan2(relativePosition.y, relativePosition.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);

        // Move in the tangent direction of the planet's surface
        Vector3 tangentDirection = Vector3.Cross(relativePosition, Vector3.forward);
        transform.position += tangentDirection.normalized * direction.x * moveSpeed * Time.deltaTime;
    }

    // Method to align the Walker's up direction relative to the planet's surface
    private void AlignToPlanetSurface()
    {
        if (planetCenter == null) return;

        // Calculate the direction from the Walker to the planet's center
        Vector3 toPlanet = planetCenter.position - transform.position;

        // Set the Walkers up direction to be opposite to the direction towards the planet's center
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, toPlanet);
        transform.rotation = targetRotation;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is colliding with the planet's surface
        if (collision.gameObject.CompareTag("Planet"))
        {
            isGrounded = true;
        }
    } 
}
