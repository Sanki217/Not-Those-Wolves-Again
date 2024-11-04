using System.Collections;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public int sheepIndex;                
    public Transform dogTransform;       
    public LayerMask groundLayer;         

    private enum SheepState
    {
        Idle,       
        Running,    
        Falling    
    }

    private SheepState currentState = SheepState.Idle; 
    private bool isInSafeZone = false;                 
    private Bounds safeZoneBounds;                     
    private bool isGrounded = true;                    
    private bool isFalling = false;                    

    [Header("Wandering Settings")]
    public float minMoveDistance = 1f;   
    public float maxMoveDistance = 3f;   
    public float minMoveInterval = 1.5f; 
    public float maxMoveInterval = 4f;   
    public float minSpeed = 1f;          
    public float maxSpeed = 3f;          

    [Header("Bark Cooldown")]
    public float barkCooldown = 2f;      

    private Rigidbody rb;
    private float lastBarkedTime;       
    private bool isWandering = false;

    private Animator sheepAnimator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        sheepAnimator = GetComponentInChildren<Animator>();
       
        float randomInitialDelay = Random.Range(2f, maxMoveInterval);
        StartCoroutine(StartWanderingAfterDelay(randomInitialDelay));
    }

    private void Update()
    {
        if (!isFalling)  
        {
            DrawLineToDog(); 
            isGrounded = IsGrounded();

            bool isMoving = rb.velocity.magnitude > 0.1f;
            sheepAnimator.SetBool("IsMoving", isMoving);


            if (rb.velocity.magnitude > 0.1f)
            {
                Vector3 direction = rb.velocity.normalized;
                direction.y = 0; 
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
            }
        }
    }


    public void OnBarkedAt(Vector3 barkForceVector)
    {
        if (isInSafeZone || isFalling) return; 

        rb.velocity = Vector3.zero; 
        rb.AddForce(barkForceVector, ForceMode.Impulse);
        lastBarkedTime = Time.time; 
        currentState = SheepState.Running; 

        StopCoroutine(WanderCoroutine());
        StartCoroutine(ResumeWanderingAfterCooldown());
    }

    
    public void SetInSafeZone(bool inZone, Bounds bounds)
    {
        isInSafeZone = inZone;
        if (isInSafeZone)
        {
            safeZoneBounds = bounds;  
        }
    }

    public bool IsInSafeZone()
    {
        return isInSafeZone;
    }

    private IEnumerator StartWanderingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        StartCoroutine(WanderCoroutine()); 
    }

    private IEnumerator WanderCoroutine()
    {
        isWandering = true;

        while (true)
        {
            if (Time.time - lastBarkedTime < barkCooldown)
            {
                currentState = SheepState.Idle;
                yield return null;
            }
            else
            {
                currentState = SheepState.Idle; 

                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                float randomDistance = Random.Range(minMoveDistance, maxMoveDistance);
                float randomSpeed = Random.Range(minSpeed, maxSpeed);

                Vector3 targetPosition = transform.position + randomDirection * randomDistance;

                if (isInSafeZone)
                {
                    targetPosition = RestrictToBounds(targetPosition, safeZoneBounds);
                }

                Vector3 targetVelocity = (targetPosition - transform.position).normalized * randomSpeed;
                targetVelocity.y = rb.velocity.y;

                float moveTime = randomDistance / randomSpeed;
                float elapsedTime = 0f;

                while (elapsedTime < moveTime)
                {
                    if (currentState == SheepState.Running || isFalling) yield break; 

                    rb.velocity = targetVelocity;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                rb.velocity = new Vector3(0, rb.velocity.y, 0);

                float waitTime = Random.Range(minMoveInterval, maxMoveInterval);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathHole"))
        {
            EnterDeathHole();  
        }
    }

   
    private void EnterDeathHole()
    {
        isFalling = true;
        currentState = SheepState.Falling;  
        rb.velocity = new Vector3(0, rb.velocity.y, 0); 
        rb.useGravity = true; 
    }

    private Vector3 RestrictToBounds(Vector3 targetPosition, Bounds bounds)
    {
        return new Vector3(
            Mathf.Clamp(targetPosition.x, bounds.min.x, bounds.max.x),
            transform.position.y, 
            Mathf.Clamp(targetPosition.z, bounds.min.z, bounds.max.z)
        );
    }

    private IEnumerator ResumeWanderingAfterCooldown()
    {
        yield return new WaitForSeconds(barkCooldown);
        if (!isInSafeZone && isGrounded && !isFalling)
        {
            currentState = SheepState.Idle; 
            StartCoroutine(WanderCoroutine()); 
        }
    }

    private bool IsGrounded()
    {
        float groundCheckDistance = 0.1f;  
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void DrawLineToDog()
    {
        if (isFalling) return;  

        float distanceToDog = Vector3.Distance(transform.position, dogTransform.position);
        Color lineColor;

       
        if (distanceToDog > 6f)
        {
            lineColor = Color.red;
        }
        else if (distanceToDog > 4f)
        {
            lineColor = Color.yellow;
        }
        else if (distanceToDog > 3f)
        {
            lineColor = Color.green;
        }
        else
        {
            lineColor = Color.blue;
        }

        Debug.DrawLine(transform.position, dogTransform.position, lineColor);
    }
}
