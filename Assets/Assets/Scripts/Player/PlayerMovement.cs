using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float smoothTime = 0.1f; // Smooth acceleration/deceleration
    public DynamicJoystick joystick;
    public AudioSource footstepsAudio;

    float horizontalInput;
    float verticalInput;

    private bool isFalling = false;
    private Rigidbody rb;
    private Vector3 currentVelocity;

    private void Start()
    {
        // Get the Rigidbody component and lock the X and Z rotation to prevent rolling
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Stops rotation altogether, but we'll control Y rotation manually
    }

    private void Update()
    {
        // Get the horizontal and vertical input from the joystick
        horizontalInput = joystick.Horizontal;
        verticalInput = joystick.Vertical;

        if (transform.position.y < 0)
        {
            StartFalling();
        }

    }
    private void FixedUpdate()
    {

        if (!isFalling)
        {



            // Calculate the movement direction (on XZ plane) and normalize it
            Vector3 targetDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            // If there is any movement, face the direction of movement
            if (targetDirection.magnitude > 0.1f) // Only rotate if there is enough input
            {
                // Rotate the player to face the target direction (Y axis only)
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
            }

            // Calculate the target velocity
            Vector3 targetVelocity = targetDirection * maxSpeed;

            // Smoothly change the velocity using SmoothDamp
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);

            // Odtwarzanie d�wi�ku krok�w, gdy pies si� porusza
            if (targetVelocity.magnitude > 0.1f && !footstepsAudio.isPlaying)
            {
                footstepsAudio.Play();
            }
            else if (targetVelocity.magnitude <= 0.1f && footstepsAudio.isPlaying)
            {
                footstepsAudio.Stop();
            }

        }
    }
    private void StartFalling()
    {
        // Stop footsteps sound when falling
        if (footstepsAudio.isPlaying)
        {
            footstepsAudio.Stop();
        }

        // Set the falling state and allow gravity to control Y movement
        isFalling = true;
    }

}