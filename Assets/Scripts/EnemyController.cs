using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movement & Knockback")]
    public float moveSpeed = 3f;
    public float knockbackForce = 25f; // How hard the enemy gets pushed back
    private bool isKnockedBack = false;
    private Vector2 knockbackVelocity;

    [Header("References")]
    private Transform playerTransform;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Find the player automatically in the scene by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        // Don't chase while undergoing knockback momentum
        if (isKnockedBack) return;

        // If the player exists, move towards them
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            // Flip the sprite based on movement direction
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Called by the weapon script when hit
    public void TakeDamage(int damageAmount, Vector2 hitSourcePosition)
    {
        currentHealth -= damageAmount;
        Debug.Log("Enemy took damage! Current health: " + currentHealth);

        // Trigger visual flash and knockback
        StartCoroutine(HitFlashRoutine());
        ApplyKnockback(hitSourcePosition);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ApplyKnockback(Vector2 hitSourcePosition)
    {
        // Calculate direction pushing away from the weapon
        Vector2 knockbackDir = ((Vector2)transform.position - hitSourcePosition).normalized;

        // Set the knockback velocity magnitude
        knockbackVelocity = knockbackDir * knockbackForce;

        // Start the routine to hold the knockback state briefly
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector2 newPosition = rb.position + -direction * (5f)*moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
        StartCoroutine(KnockbackRoutine());
    }

    IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;

        // How long the enemy is stunned and sliding backward (tweak this if needed)
        yield return new WaitForSeconds(0.18f);

        // Clear knockback velocity so they don't slide forever
        rb.linearVelocity = Vector2.zero; // Use rb.velocity if on Unity 2022 or older
        isKnockedBack = false;
    }

    IEnumerator HitFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            // Multiply the original color by a brightness factor (e.g., 2.5 makes it 250% brighter)
            // If you want it extremely bright/glowing, try 3.0 or 4.0
            Color brightColor = originalColor * 3.0f;

            // Keep the alpha (transparency) the same as original so it doesn't become opaque weirdly
            brightColor.a = originalColor.a;

            spriteRenderer.color = brightColor;

            yield return new WaitForSeconds(0.15f); // Duration of the flash

            // Return to original color
            spriteRenderer.color = originalColor;
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}