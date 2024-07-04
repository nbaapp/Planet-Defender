using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Unit
{
    public Transform planet; // Assign the planet in the Inspector
    public float speed = 5f; // Speed of player movement
    public float maxHorizontalSpeed = 10f; // Maximum horizontal speed cap
    public float jumpForce = 10f; // Force applied for jumping
    public float gravityForce = 9.8f; // Gravity force pulling the player towards the planet
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private PlayerInputActions inputActions;

    private bool isGrounded; // Check if the player is grounded
    private Vector2 directionToPlanet; // Cached direction to the planet
    private Vector2 tangentialDirection; // Cached tangential direction

    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public Transform firePoint; // The point from where the bullets are fired
    public float fireRate = 0.1f; // Time between shots for rapid fire
    private Vector2 aimDirection;
    private bool isAttacking;
    private float lastFireTime;
    public HealthBar healthBar;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthBar.SetMaxHealth(maxHealth);

        // Initialize the input actions
        inputActions = new PlayerInputActions();        
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Aim.performed += OnAim;
        inputActions.Player.Aim.canceled += OnAim;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Aim.performed -= OnAim;
        inputActions.Player.Aim.canceled -= OnAim;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
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
        if (isAttacking && Time.time - lastFireTime >= fireRate && !logic.IsBuildMode())
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

    public override void TakeDamage(int damage)
    {
        // Call the base class method to handle the damage
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        // Read the input value from the context
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            // Apply an upward force relative to the direction to the planet
            Vector2 jumpDirection = -directionToPlanet;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Calculate and cache the direction to the planet and tangential direction
        CalculateDirections();

        // Handle the player movement
        HandleMovement();

        // Cap the horizontal speed to the maximum speed
        CapHorizontalSpeed();
    }

    // Method to calculate and cache direction vectors
    private void CalculateDirections()
    {
        // Calculate the direction to the planet
        directionToPlanet = (planet.position - transform.position).normalized;

        // Calculate the right vector relative to the planet's surface (tangential direction)
        tangentialDirection = new Vector2(-directionToPlanet.y, directionToPlanet.x);

        // Rotate the player to face away from the planet center
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(angle);
    }

    // Method to handle the movement logic
    private void HandleMovement()
    {
        // Move the player along the surface of the planet
        rb.AddForce(tangentialDirection * moveInput.x * speed, ForceMode2D.Force);
    }

    // Method to cap the player's horizontal speed relative to the planet
    private void CapHorizontalSpeed()
    {
        // Project the player's velocity onto the tangential direction
        float horizontalSpeed = Vector2.Dot(rb.velocity, tangentialDirection);

        // If the horizontal speed exceeds the max horizontal speed, cap it
        if (Mathf.Abs(horizontalSpeed) > maxHorizontalSpeed)
        {
            // Calculate the capped velocity component in the tangential direction
            Vector2 cappedHorizontalVelocity = tangentialDirection * maxHorizontalSpeed * Mathf.Sign(horizontalSpeed);

            // Combine the capped horizontal velocity with the vertical component of the original velocity
            Vector2 verticalComponent = rb.velocity - (horizontalSpeed * tangentialDirection);
            rb.velocity = cappedHorizontalVelocity + verticalComponent;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is colliding with the planet's surface
        if (collision.gameObject.CompareTag("Planet"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the player is no longer in contact with the planet's surface
        if (collision.gameObject.CompareTag("Planet"))
        {
            isGrounded = false;
        }
    }
}
