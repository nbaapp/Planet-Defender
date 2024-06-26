using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Initialize the input actions
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        // Enable the input actions and set up the callback for the Move and Jump actions
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        // Disable the input actions and remove the callback
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Disable();
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
