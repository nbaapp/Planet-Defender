using UnityEngine;

public class Walker : Alien
{
    private Rigidbody2D rb;
    private bool hasLanded = false;
    public float minFallAngle = 70f; // Minimum angle relative to the planet's surface
    public float maxFallAngle = 110f; // Maximum angle relative to the planet's surface
    public float initialFallForce = 5f; // Initial force applied when spawning

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        // Apply an initial downward force to simulate falling from the sky
        ApplyInitialFall();

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Move();
    }

    private void Move()
    {
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

    protected override void Die()
    {
        // Add specific death behavior for Walker, like playing an animation
        // Call the base method to destroy the object
        base.Die();
    }
}
