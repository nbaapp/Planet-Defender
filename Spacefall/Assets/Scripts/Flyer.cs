using UnityEngine;

public class Flyer : Alien
{
    public float hoverHeightMin = 5f; // Minimum hover height above the planet
    public float hoverHeightMax = 10f; // Maximum hover height above the planet
    public float bobbingAmplitude = 0.5f; // Amplitude of the bobbing effect
    public float bobbingSpeed = 2f; // Speed of the bobbing effect
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public float attackRange = 10f; // Range within which the Flyer can attack
    public float attackInterval = 2f; // Interval between attacks

    private float hoverHeight;
    private float lastAttackTime;
    private Vector3 initialPosition;

    protected override void Start()
    {
        base.Start();
        hoverHeight = Random.Range(hoverHeightMin, hoverHeightMax);
        initialPosition = transform.position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Hover();

        // Check if it's time to attack
        if (Time.time > lastAttackTime + attackInterval)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Hover()
    {
        if (planetCenter == null) return;

        // Calculate the hover position
        Vector3 toPlanet = planetCenter.position - transform.position;
        Vector3 hoverPosition = planetCenter.position + toPlanet.normalized * (toPlanet.magnitude - hoverHeight);

        // Apply the bobbing effect
        hoverPosition += Vector3.up * Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;

        // Move towards the hover position
        transform.position = Vector3.Lerp(transform.position, hoverPosition, moveSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        Transform target = FindNearestTarget();
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;

        // Instantiate and shoot the bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.SetVelocity(direction);
            bulletComponent.SetRotation(direction);
        }
    }

    private Transform FindNearestTarget()
    {
        float closestDistance = float.MaxValue;
        Transform nearestTarget = null;

        // Check for nearest city within range
        foreach (var city in FindObjectsOfType<City>())
        {
            float distance = Vector2.Distance(transform.position, city.transform.position);
            if (distance < attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = city.transform;
            }
        }

        // Check for nearest player within range
        var player = FindObjectOfType<PlayerMovement>(); // Assuming you have a PlayerMovement script
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                nearestTarget = player.transform;
            }
        }

        return nearestTarget;
    }

    protected override void Die()
    {
        // Add specific death behavior for Flyer, like playing an animation
        // Call the base method to destroy the object
        base.Die();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (planetCenter != null)
        {
            // Draw the min hover height range
            Gizmos.DrawWireSphere(planetCenter.position, Vector3.Distance(planetCenter.position, transform.position) - hoverHeightMin);

            // Draw the max hover height range
            Gizmos.DrawWireSphere(planetCenter.position, Vector3.Distance(planetCenter.position, transform.position) - hoverHeightMax);
        }
    }
}
