using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    public WeaponSwing weapon;


    private KeyCode lastKeyPressed;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.25f;
    private Vector2 dodgeDirection;

    private string lastDirection = "Down"; // Track last faced direction
    private float idleTimer = 0f; 
    private bool isIdleRuleTriggered = false;
    //private bool mouseDown = false;
    //private float mouseTimer = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //weapon = GetComponent<WeaponSwing>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("Attack");
        
        if (isAttacking) {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false);

            return;
        
        }
        

        if (weapon.startSpecial)
        {
            TriggerAttack();
            return;
        }
        else {
            animator.SetBool("SpecialAttack", false);
        }

        

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
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SpecialAttack")) return;
        
        // Move the player
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        CheckDoubleTap("Up", Vector2.up);
        CheckDoubleTap("Down", Vector2.down);
        CheckDoubleTap("Left", Vector2.left);
        CheckDoubleTap("Right", Vector2.right);

        
        

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

    void TriggerAttack()
    {
        // Trigger the animation using the exact name of your Animator parameter
        animator.SetBool("SpecialAttack", true);
        animator.SetTrigger("Attack");

    }

    void CheckDoubleTap(string testKey, Vector2 dir)
    {
        bool isLastDirection = testKey == lastDirection;
        UnityEngine.Debug.Log(isLastDirection + " and "+Input.GetKeyDown(KeyCode.E));
        if ((isLastDirection) && Input.GetKeyDown(KeyCode.E))
        {

            // If key is pressed twice within threshold time
            rb.MovePosition(rb.position + dir * 1.2f);
        }
    }




}
