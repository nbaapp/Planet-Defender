using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 10f; // Detection range for enemies
    public float fireRate = 1f; // Time between shots
    public float projectileSpeed = 10f; // Speed of the fired projectile
    public GameObject projectilePrefab; // Prefab of the projectile
    public LayerMask enemyLayer; // Layer of enemies

    private Transform target; // Current target
    private float fireCooldown = 0f; // Cooldown timer for shooting

    void Update()
    {
        FindNearestEnemy();
        
        if (target != null)
        {
            AimAtTarget();
            
            if (fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = 1f / fireRate; // Reset the cooldown based on fire rate
            }
            
            fireCooldown -= Time.deltaTime;
        }
    }

    void FindNearestEnemy()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        
        foreach (var enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null && closestDistance <= range)
        {
            target = closestEnemy;
        }
        else
        {
            target = null; // No enemies within range
        }
    }

    void AimAtTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Shoot()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Bullet bullet = projectile.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetVelocity(transform.right * projectileSpeed);
            }
        }
    }

    // Optional: Draw the detection range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
