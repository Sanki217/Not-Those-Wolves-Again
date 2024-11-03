using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float smoothTime = 0.1f; 
    public DynamicJoystick joystick;
    public AudioSource footstepsAudio;

    float horizontalInput;
    float verticalInput;

    private bool isFalling = false;
    private Rigidbody rb;
    private Vector3 currentVelocity;
    private Animator animator; 

    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); 

        rb.freezeRotation = true; 
    }

    private void Update()
    {
        
        horizontalInput = joystick.Horizontal;
        verticalInput = joystick.Vertical;

        if (transform.position.y < 0)
        {
            StartFalling();
        }

       
        float speed = new Vector3(horizontalInput, 0, verticalInput).magnitude * maxSpeed;
        animator.SetFloat("Speed", speed); 

       
        if (speed > 0.1f && !footstepsAudio.isPlaying)
        {
            footstepsAudio.Play();
        }
        else if (speed <= 0.1f && footstepsAudio.isPlaying)
        {
            footstepsAudio.Stop();
        }
    }

    private void FixedUpdate()
    {
        
        Vector3 targetVelocity = new Vector3(horizontalInput * maxSpeed, rb.velocity.y, verticalInput * maxSpeed);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);

        
        if (targetVelocity.x != 0 || targetVelocity.z != 0)
        {
            Vector3 direction = new Vector3(targetVelocity.x, 0, targetVelocity.z);
            transform.forward = direction;
        }
    }

    private void StartFalling()
    {
        if (!isFalling)
        {
            isFalling = true;
            animator.SetTrigger("Fall"); 
        }
    }
}
