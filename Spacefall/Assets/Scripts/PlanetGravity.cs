using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravityForce = 10f;
    public float gravityRadius = 20f;
    public string gravityTag = "AffectedByGravity"; // Tag to identify objects affected by gravity

    void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        // Find all objects within the gravity radius
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, gravityRadius);

        foreach (Collider2D col in objectsInRange)
        {
            // Check if the object has the specific tag
            if (col.CompareTag(gravityTag))
            {
                Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Apply gravity towards the center of the planet
                    Vector2 direction = (Vector2)transform.position - rb.position;
                    rb.AddForce(direction.normalized * gravityForce);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the gravity radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
    }
}
