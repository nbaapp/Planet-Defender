using UnityEngine;

public class Walker : Alien
{
    private Rigidbody2D rb;
    private bool hasLanded;
    public float initialFallForce = 10f; // The force applied to simulate falling
    public float minFallAngle = -15f; // Minimum angle for the initial fall direction
    public float maxFallAngle = 15f; // Maximum angle for the initial fall direction
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public float attackRange = 10f; // Range within which the Walker can attack
    public float attackInterval = 2f; // Interval between attacks

    private float lastAttackTime;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        ApplyInitialFall();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!hasLanded)
        {
            // Check if the Walker has landed
            if (IsGrounded())
            {
                hasLanded = true;
                rb.velocity = Vector2.zero; // Stop any residual motion
                FindNearestCity(); // Find the nearest city to move towards
            }
        }
        else
        {
            // Move towards the city after landing
            MoveTowardsCity();

            // Check if it's time to attack
            if (Time.time > lastAttackTime + attackInterval)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    // Check if the walker has landed on the ground
    private bool IsGrounded()
    {
        return isGrounded;
    }

    protected void MoveTowardsCity()
    {
        if (targetCity == null || planetCenter == null) return;

        Vector3 toCity = targetCity.position - transform.position;
        Vector3 toPlanet = planetCenter.position - transform.position;
        Vector3 tangent = Vector3.Cross(toPlanet, Vector3.forward).normalized;

        // Determine if the city is to the left or right
        float direction = Vector3.Dot(tangent, toCity) > 0 ? 1f : -1f;

        // Move around the planet towards the city
        MoveAroundPlanet(new Vector2(direction, 0));
    }

    private void ApplyInitialFall()
    {
        if (planetCenter == null) return;

        // Calculate the direction from the Walker to the planet's center
        Vector3 toPlanet = planetCenter.position - transform.position;

        // Generate a random angle within the specified range
        float randomAngle = Random.Range(minFallAngle, maxFallAngle);

        // Rotate the initial fall direction by the random angle
        Vector3 fallDirection = Quaternion.Euler(0, 0, randomAngle) * -toPlanet.normalized;

        // Apply the initial force to the Rigidbody
        rb.AddForce(fallDirection * initialFallForce, ForceMode2D.Impulse);
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
        var player = FindObjectOfType<Player>(); // Assuming you have a PlayerMovement script
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
        // Add specific death behavior for Walker, like playing an animation
        // Call the base method to destroy the object
        base.Die();
    }
}
