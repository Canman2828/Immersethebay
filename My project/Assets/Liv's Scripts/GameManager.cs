using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TowerManager towerManager;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CardManager cardManager;

    private bool gameActive = false;
    private bool gameOver = false;
    private bool gameWon = false;

    private void Start()
    {
        // Find managers if not assigned
        if (towerManager == null) towerManager = FindObjectOfType<TowerManager>();
        if (enemySpawner == null) enemySpawner = FindObjectOfType<EnemySpawner>();
        if (cardManager == null) cardManager = FindObjectOfType<CardManager>();

        // Verify all managers are present
        if (towerManager == null) Debug.LogError("TowerManager not found!");
        if (enemySpawner == null) Debug.LogError("EnemySpawner not found!");
        if (cardManager == null) Debug.LogError("CardManager not found!");

        StartGame();
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame()
    {
        gameActive = true;
        gameOver = false;
        gameWon = false;

        Debug.Log("=== GAME STARTED ===");
        Debug.Log("Spawn enemies and defend your tower for 3 minutes!");
    }

    /// <summary>
    /// Called when the player loses
    /// </summary>
    public void OnGameOver()
    {
        gameActive = false;
        gameOver = true;
        gameWon = false;

        Debug.Log("=== GAME OVER ===");
        Debug.Log("Your tower was destroyed!");

        // Stop spawning
        if (enemySpawner != null)
        {
            enemySpawner.SetActive(false);
        }

        // Disable card throwing
        CardThrowHandler handler = FindObjectOfType<CardThrowHandler>();
        if (handler != null)
        {
            handler.SetEnabled(false);
        }
    }

    /// <summary>
    /// Called when the player wins
    /// </summary>
    public void OnGameWon()
    {
        gameActive = false;
        gameOver = false;
        gameWon = true;

        Debug.Log("=== GAME WON ===");
        Debug.Log("Your tower survived the wave!");

        // Stop spawning
        if (enemySpawner != null)
        {
            enemySpawner.SetActive(false);
        }

        // Disable card throwing
        CardThrowHandler handler = FindObjectOfType<CardThrowHandler>();
        if (handler != null)
        {
            handler.SetEnabled(false);
        }
    }

    /// <summary>
    /// Called when the wave completes
    /// </summary>
    public void OnWaveComplete()
    {
        if (towerManager != null)
        {
            towerManager.OnWaveComplete();
        }
    }

    /// <summary>
    /// Check if game is active
    /// </summary>
    public bool IsGameActive()
    {
        return gameActive;
    }

    /// <summary>
    /// Check if game is over
    /// </summary>
    public bool IsGameOver()
    {
        return gameOver;
    }

    /// <summary>
    /// Check if game is won
    /// </summary>
    public bool IsGameWon()
    {
        return gameWon;
    }

    /// <summary>
    /// Get game progress
    /// </summary>
    public float GetGameProgress()
    {
        if (enemySpawner != null)
        {
            return enemySpawner.GetGameProgress();
        }
        return 0f;
    }

    /// <summary>
    /// Get remaining time
    /// </summary>
    public float GetRemainingTime()
    {
        if (enemySpawner != null)
        {
            return enemySpawner.GetRemainingTime();
        }
        return 0f;
    }

    /// <summary>
    /// Get tower manager
    /// </summary>
    public TowerManager GetTowerManager()
    {
        return towerManager;
    }

    /// <summary>
    /// Get card manager
    /// </summary>
    public CardManager GetCardManager()
    {
        return cardManager;
    }

    /// <summary>
    /// Get enemy spawner
    /// </summary>
    public EnemySpawner GetEnemySpawner()
    {
        return enemySpawner;
    }
}