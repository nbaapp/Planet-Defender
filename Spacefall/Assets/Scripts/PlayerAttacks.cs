using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public Transform firePoint; // The point from where the bullets are fired
    public float fireRate = 0.1f; // Time between shots for rapid fire
    private PlayerInputActions inputActions;
    private Vector2 aimDirection;
    private bool isAttacking;
    private float lastFireTime;

    void Awake()
    {
        // Initialize the input actions
        inputActions = new PlayerInputActions();

        // Set up the callback for the Aim action
        inputActions.Player.Aim.performed += OnAim;
        inputActions.Player.Aim.canceled += OnAim;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Update()
    {
        // Handle the attack logic
        HandleAttack();
    }

    void HandleAttack()
    {
        // Check if the attack button is held down
        isAttacking = inputActions.Player.Attack.IsPressed();

        // If attacking and enough time has passed since the last shot, fire again
        if (isAttacking && Time.time - lastFireTime >= fireRate)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void OnAim(InputAction.CallbackContext context)
    {
        // Read the aim input
        Vector2 input = context.ReadValue<Vector2>();

        // Determine the aim direction based on input device
        if (context.control.device.name == "Mouse")
        {
            // Convert mouse position to world point
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            // Calculate direction from player to mouse position
            aimDirection = (worldPoint - transform.position).normalized;
        }
        else
        {
            // Use the joystick direction directly
            aimDirection = input.normalized;
        }
    }

    void Shoot()
    {
        if (aimDirection == Vector2.zero)
            return;

        // Calculate the firing direction based on the aim direction
        Vector2 fireDirection = aimDirection.normalized;

        // Instantiate the bullet at the fire point
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Get the Bullet component and configure it
        Bullet bullet = bulletObject.GetComponent<Bullet>();

        // Set the bullet's velocity and rotation
        bullet.SetVelocity(fireDirection);
        bullet.SetRotation(fireDirection);
    }
}
