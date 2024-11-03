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
    public Animator animator;
   

    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>(); 

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

       
     

        float movementMagnitude = new Vector3(horizontalInput, 0, verticalInput).magnitude;
        animator.SetFloat("Speed", movementMagnitude); // Set speed in Animator to control animations


    }

    private void FixedUpdate()
    {

        if (!isFalling)
        {



            Vector3 targetDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (targetDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            Vector3 targetVelocity = targetDirection * maxSpeed;

            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, smoothTime);

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
        if (footstepsAudio.isPlaying)
        {
            footstepsAudio.Stop();
        }

        isFalling = true;
    }

}
