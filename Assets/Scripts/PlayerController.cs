using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    private string lastDirection = "Down"; // Track last faced direction
    private float idleTimer = 0f; 
    private bool isIdleRuleTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get WASD Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize movement vector to prevent faster diagonal movement
        if (movement.magnitude > 0)
        {
            movement = movement.normalized;

            // Track the primary direction the player is facing based on input
            TrackDirection(movement);

            // Feed inputes into the blend tree
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("IsMoving", true);

            // Reset idle timer and rule trigger
            idleTimer = 0f;
            isIdleRuleTriggered = false;
        }
        else
        {
            //Player is idle
            animator.SetBool("IsMoving", false);

            // Run idle timer logic
            if (!isIdleRuleTriggered)
            {
                idleTimer += Time.deltaTime;

                if (idleTimer >= 5f) // 5 seconds of inactivity
                {
                    TriggerCustomIdle();
                    isIdleRuleTriggered = true; // Prevent retriggering
                }
                else
                {
                    // Instant idle animation trigger
                    TriggerImmediateIdle();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void TrackDirection(Vector2 move)
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            lastDirection = move.x > 0 ? "Right" : "Left";
        }
        else if (Mathf.Abs(move.y) > Mathf.Abs(move.x))
        {
            lastDirection = move.y > 0 ? "Up" : "Down";
        }
    }

    void TriggerImmediateIdle()
    {
        // Standard behavior before 5 seconds hit
        if (lastDirection == "Left") animator.Play("DogIdleLeft");
        if (lastDirection == "Right") animator.Play("DogIdleRight");
    }

    void TriggerCustomIdle()
    {
        // Your custom 5-second timeout rules:
        if (lastDirection == "Up")
        {
            animator.Play("Idle Right");
        }
        else if (lastDirection == "Down")
        {
            animator.Play("Idle Left");
        }
        else if (lastDirection == "Left")
        {
            animator.Play("Idle Left");
        }
        else if (lastDirection == "Right")
        {
            animator.Play("Idle Right");
        }
    }
}
