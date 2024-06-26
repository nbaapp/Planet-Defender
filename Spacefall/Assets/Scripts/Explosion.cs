using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    
    public float duration = 1f; // Duration of the explosion effect
    public float radius = 1f;
    public float force = 10f;
    public int damage = 50;
    public LayerMask enemyLayer;
    private CameraFollow cam;
    private CircleCollider2D circleCollider;

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;
    private List<Alien> affectedEnemies = new List<Alien>();

    void Awake()
    {
        // Get the CircleCollider2D component
        circleCollider = GetComponent<CircleCollider2D>();
        cam = Camera.main.GetComponent<CameraFollow>();

        // Ensure the collider is a trigger so it doesn't interfere with physics
        circleCollider.isTrigger = true;
    }

    void Start()
    {
        CameraShake();
        // Start the explosion effect
        StartCoroutine(HandleExplosion());
    }

    private IEnumerator HandleExplosion()
    {
        // Detect enemies within the explosion radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            Alien enemy = hitCollider.GetComponent<Alien>();
            if (enemy != null && !affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                ApplyExplosionEffects(enemy);
            }
        }

        yield return new WaitForSeconds(duration);

        // Destroy the explosion effect after the duration
        Destroy(gameObject);
    }

    private void ApplyExplosionEffects(Alien enemy)
    {
        // Calculate the direction from the explosion to the enemy
        Vector2 direction = (enemy.transform.position - transform.position).normalized;
        
        // Apply knockback
        enemy.TakeKnockback(direction, force);
        
        // Apply damage
        enemy.TakeDamage(damage);
    }

    private void CameraShake()
    {
        if (cam != null)
        {
            cam.StartShakeCamera(shakeDuration, shakeMagnitude);
        }
    }

    //draw the explosion radius in the editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
}
