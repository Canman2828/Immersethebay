using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject werewolfPrefab;
    [SerializeField] private GameObject harpyPrefab;
    [SerializeField] private GameObject cerberusPrefab;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<Vector3> pathPoints = new List<Vector3>();

    private float gameDuration = 180f; // 3 minutes in seconds
    private float elapsedTime = 0f;
    private bool gameActive = true;

    // Enemy spawn configuration
    [SerializeField] private float initialSpawnRate = 1.5f; // Seconds between spawns at start
    [SerializeField] private float minSpawnRate = 0.3f; // Fastest spawn rate at end
    private float currentSpawnRate;
    private float timeSinceLastSpawn = 0f;

    // Wave configuration
    private int currentWave = 0;
    private float waveChangeDuration = 30f; // Change enemy types every 30 seconds
    private float timeSinceWaveStart = 0f;

    // Enemy type ratios that change over time
    private float werewolfRatio = 0.7f;
    private float harpyRatio = 0.25f;
    private float cerberusRatio = 0.05f;

    private int totalEnemiesSpawned = 0;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point not assigned to EnemySpawner!");
        }

        if (pathPoints.Count == 0)
        {
            Debug.LogWarning("No path points assigned to EnemySpawner. Enemies will spawn at origin.");
        }

        currentSpawnRate = initialSpawnRate;
    }

    private void Update()
    {
        if (!gameActive) return;

        elapsedTime += Time.deltaTime;
        timeSinceLastSpawn += Time.deltaTime;
        timeSinceWaveStart += Time.deltaTime;

        // Check if game time is up
        if (elapsedTime >= gameDuration)
        {
            EndGame();
            return;
        }

        // Update enemy distribution based on elapsed time
        UpdateEnemyDistribution();

        // Update spawn rate based on elapsed time (progressive difficulty)
        UpdateSpawnRate();

        // Spawn new enemies
        if (timeSinceLastSpawn >= currentSpawnRate)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    /// <summary>
    /// Updates the spawn rate progressively over time
    /// </summary>
    private void UpdateSpawnRate()
    {
        // Lerp from initial to minimum spawn rate over the game duration
        float progress = elapsedTime / gameDuration;
        currentSpawnRate = Mathf.Lerp(initialSpawnRate, minSpawnRate, progress);
    }

    /// <summary>
    /// Updates enemy type distribution based on game progress
    /// </summary>
    private void UpdateEnemyDistribution()
    {
        float progress = elapsedTime / gameDuration;

        // First 30 seconds: mostly werewolves
        if (progress < 0.17f)
        {
            werewolfRatio = 0.8f;
            harpyRatio = 0.2f;
            cerberusRatio = 0.0f;
        }
        // 30-60 seconds: introduce harpies
        else if (progress < 0.33f)
        {
            werewolfRatio = 0.6f;
            harpyRatio = 0.35f;
            cerberusRatio = 0.05f;
        }
        // 60-120 seconds: balanced mix
        else if (progress < 0.67f)
        {
            werewolfRatio = 0.5f;
            harpyRatio = 0.35f;
            cerberusRatio = 0.15f;
        }
        // 120-180 seconds: increased difficulty, more harpies and cerberus
        else
        {
            werewolfRatio = 0.3f;
            harpyRatio = 0.4f;
            cerberusRatio = 0.3f;
        }
    }

    /// <summary>
    /// Spawn a single enemy
    /// </summary>
    private void SpawnEnemy()
    {
        GameObject enemyPrefab = SelectEnemyType();

        if (enemyPrefab == null)
        {
            Debug.LogError("No enemy prefab selected!");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Assign path to opponent
        Opponent opponent = enemyInstance.GetComponent<Opponent>();
        if (opponent != null)
        {
            opponent.SetPath(pathPoints);
            totalEnemiesSpawned++;
            Debug.Log($"Spawned enemy #{totalEnemiesSpawned}: {enemyPrefab.name}");
        }
        else
        {
            Debug.LogError($"Enemy prefab {enemyPrefab.name} missing Opponent component!");
            Destroy(enemyInstance);
        }
    }

    /// <summary>
    /// Select which enemy type to spawn based on ratios
    /// </summary>
    private GameObject SelectEnemyType()
    {
        float random = Random.Range(0f, 1f);

        if (random < werewolfRatio)
        {
            return werewolfPrefab;
        }
        else if (random < werewolfRatio + harpyRatio)
        {
            return harpyPrefab;
        }
        else
        {
            return cerberusPrefab;
        }
    }

    /// <summary>
    /// Set the path that enemies should follow
    /// </summary>
    public void SetEnemyPath(List<Vector3> newPath)
    {
        pathPoints = new List<Vector3>(newPath);
    }

    /// <summary>
    /// End the game/wave
    /// </summary>
    private void EndGame()
    {
        gameActive = false;
        Debug.Log($"Wave complete! Total enemies spawned: {totalEnemiesSpawned}");

        // Signal game manager that the wave is complete
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.OnWaveComplete();
        }
    }

    /// <summary>
    /// Pause/resume spawning
    /// </summary>
    public void SetActive(bool active)
    {
        gameActive = active;
    }

    /// <summary>
    /// Get the current game progress (0 to 1)
    /// </summary>
    public float GetGameProgress()
    {
        return Mathf.Clamp01(elapsedTime / gameDuration);
    }

    /// <summary>
    /// Get remaining time in seconds
    /// </summary>
    public float GetRemainingTime()
    {
        return Mathf.Max(0, gameDuration - elapsedTime);
    }

    /// <summary>
    /// Get current spawn rate (for UI display)
    /// </summary>
    public float GetCurrentSpawnRate()
    {
        return currentSpawnRate;
    }

    /// <summary>
    /// Get total enemies spawned so far
    /// </summary>
    public int GetTotalEnemiesSpawned()
    {
        return totalEnemiesSpawned;
    }
}