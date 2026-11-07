using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;      // Drag your Enemy prefab here
    public float spawnInterval = 2f;    // Time in seconds between each spawn
    public float minSpawnDistance = 3f; // Minimum distance away from the player
    public float maxSpawnDistance = 10f;// Maximum distance away from the player
    public float enemyCounter = 0;

    private Transform playerTransform;
    private float spawnTimer;

    void Start()
    {
        // Automatically find the player by tag or component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("EnemySpawner: No GameObject with tag 'Player' found in the scene!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Count down the timer
        if (enemyCounter < 7)
        {
            
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                enemyCounter += 1;
                SpawnEnemy();
                spawnTimer = 0f; // Reset timer
            }
        }
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < 10; i++)
        {
            // 1. Pick a random direction (angle) in 2D space
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 2. Pick a random distance between min and max range
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

            // 3. Calculate spawn position relative to the player's current position
            Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * randomDistance);

            // Optional: Keep Z at 0 for 2D games
            spawnPosition.z = 0f;

            // 4. Instantiate/spawn the enemy
            if (enemyPrefab != null)
            {

                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            }
            else
            {
                Debug.LogWarning("EnemySpawner: Enemy Prefab is missing!");
            }
        }
    }
}