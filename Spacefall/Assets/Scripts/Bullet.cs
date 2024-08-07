using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Speed of the bullet
    public int bulletDamage = 10; // Damage dealt by the bullet
    public float lifetime = 5f; // Time in seconds before the bullet is destroyed
    public GameObject burstEffectPrefab; // Burst effect prefab to instantiate on hit

    private Rigidbody2D rb;
    private Collider2D bulletCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Set initial velocity if not set by external script
        rb.velocity = transform.right * speed;

        // Schedule the bullet to be destroyed after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    public void SetVelocity(Vector2 direction)
    {
        // Set the bullet's velocity based on the given direction
        rb.velocity = direction.normalized * speed;
    }

    public void SetRotation(Vector2 direction)
    {
        // Set the bullet's rotation to match the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (burstEffectPrefab != null)
        {
            Instantiate(burstEffectPrefab, transform.position, Quaternion.identity);
        }

        Unit unit = collision.gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(bulletDamage);
        }

        City city = collision.gameObject.GetComponent<City>();
        if (city != null)
        {
            city.TakeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
    
}
