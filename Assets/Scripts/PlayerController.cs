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

        if (isAttacking)
        {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        if (weapon != null && weapon.startSpecial)
        {
            TriggerAttack();
            return;
        }
        else if (weapon != null)
        {
            animator.SetBool("SpecialAttack", false);
        }

        // Get WASD Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize movement vector to prevent faster diagonal movement
        if (movement.magnitude > 0)
        {
            movement = movement.normalized;
            TrackDirection(movement);

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("IsMoving", true);

            idleTimer = 0f;
            isIdleRuleTriggered = false;
        }
        else
        {
            animator.SetBool("IsMoving", false);

            if (!isIdleRuleTriggered)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= 5f)
                {
                    TriggerCustomIdle();
                    isIdleRuleTriggered = true;
                }
                else
                {
                    TriggerImmediateIdle();
                }
            }
        }

        // Check for Dodge/Key press in Update (prevents missed inputs)
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformDodge();
        }
    }

    void FixedUpdate()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SpecialAttack")) return;

        // Move the player normally via physics
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void PerformDodge()
    {
        Vector2 dodgeDir = Vector2.down;

        if (lastDirection == "Up") dodgeDir = Vector2.up;
        else if (lastDirection == "Down") dodgeDir = Vector2.down;
        else if (lastDirection == "Left") dodgeDir = Vector2.left;
        else if (lastDirection == "Right") dodgeDir = Vector2.right;

        // Execute the dodge step in the direction the player is facing
        rb.MovePosition(rb.position + dodgeDir * 1.2f);
        Debug.Log("Dodge executed in direction: " + lastDirection);
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
        if (lastDirection == "Up" || lastDirection == "Right")
        {
            animator.Play("DogIdleRight"); // Fallback to a verified state name
        }
        else
        {
            animator.Play("DogIdleLeft");  // Fallback to a verified state name
        }
    }

    void TriggerAttack()
    {
        // Trigger the animation using the exact name of your Animator parameter
        animator.SetBool("SpecialAttack", true);
        animator.SetTrigger("Attack");

    }
}