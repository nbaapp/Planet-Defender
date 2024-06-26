using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player transform in the Inspector
    public Transform planet; // Assign the planet transform in the Inspector
    public float followSpeed = 5f; // Speed at which the camera follows the player tangentially
    public float rotationSpeed = 5f; // Speed at which the camera rotates to align with the planet
    public float distanceFromPlanet = 20f; // Fixed distance from the planet center to the camera

    private Vector3 shakeOffset;
    private bool isShaking = false;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float dampingSpeed = 1.0f;

    private void FixedUpdate()
    {
        TrackPlayer();

        if (isShaking)
        {
            ApplyShakeEffect();
        }
    }

    private void TrackPlayer()
    {
        if (player == null || planet == null)
            return;

        // Calculate the direction from the planet's center to the player
        Vector3 directionToPlayer = (player.position - planet.position).normalized;

        // Calculate the camera's position directly above the player at the specified distance from the planet
        Vector3 desiredPosition = planet.position + directionToPlayer * distanceFromPlanet;
        desiredPosition.z = transform.position.z; // Maintain the fixed Z position

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Calculate the desired rotation so that the camera's up direction matches the player's up direction
        Quaternion desiredRotation = Quaternion.LookRotation(Vector3.forward, directionToPlayer);

        // Smoothly rotate the camera to the desired rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyShakeEffect()
    {
        if (shakeDuration > 0)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0; // Keep the shake effect on the x and y axes only
            shakeDuration -= Time.deltaTime * dampingSpeed;

            // Apply shake offset to the current position
            transform.position += shakeOffset;
        }
        else
        {
            isShaking = false;
            shakeDuration = 0f;
            shakeOffset = Vector3.zero;
        }
    }

    // Method to trigger the camera shake
    public void StartShakeCamera(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        isShaking = true;
    }
}
