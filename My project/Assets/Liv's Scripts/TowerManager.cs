using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.ComponentModel;

public class TowerManager : MonoBehaviour
{
    [SerializeField] private float maxHealth = 120f;
    private float currentHealth;

    [SerializeField] private Image healthFillImage;
    [SerializeField] private Canvas worldCanvas; // For world-space UI

    private List<Ally> currentAllies = new List<Ally>();
    private bool gameOver = false;
    private bool gameWon = false;

    private GameManager gameManager;

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = FindObjectOfType<GameManager>();
        UpdateHealthDisplay();

        if (healthFillImage == null)
        {
            healthFillImage = GetComponentInChildren<Image>();
        }
    }

    /// <summary>
    /// Damage the tower
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (gameOver || gameWon) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthDisplay();
        Debug.Log($"Tower took {damage} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            GameOver(false);
        }
    }

    /// <summary>
    /// Heal the tower
    /// </summary>
    public float Heal(float amount)
    {
        float oldHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        float actualHealed = currentHealth - oldHealth;
        UpdateHealthDisplay();

        Debug.Log($"Tower healed for {actualHealed}. Health: {currentHealth}/{maxHealth}");
        return actualHealed;
    }

    /// <summary>
    /// Register an ally with the tower
    /// </summary>
    public void RegisterAlly(Ally ally)
    {
        if (!currentAllies.Contains(ally))
        {
            currentAllies.Add(ally);
            Debug.Log($"Ally registered. Total allies: {currentAllies.Count}");
        }
    }

    /// <summary>
    /// Unregister an ally (when it dies or despawns)
    /// </summary>
    public void UnregisterAlly(Ally ally)
    {
        if (currentAllies.Contains(ally))
        {
            currentAllies.Remove(ally);
            Debug.Log($"Ally unregistered. Total allies: {currentAllies.Count}");
        }
    }

    /// <summary>
    /// Get the list of current allies
    /// </summary>
    public List<Ally> GetCurrentAllies()
    {
        return new List<Ally>(currentAllies);
    }

    /// <summary>
    /// Get current health
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Get max health
    /// </summary>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Get health as a percentage (0 to 1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    /// <summary>
    /// Update the health fill image
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    /// <summary>
    /// Handle game over
    /// </summary>
    private void GameOver(bool victory)
    {
        gameOver = !victory;
        gameWon = victory;

        if (victory)
        {
            Debug.Log("GAME WON! Tower survived the wave!");
        }
        else
        {
            Debug.Log("GAME OVER! Tower destroyed!");
        }

        // Signal game manager
        if (gameManager != null)
        {
            if (victory)
                gameManager.OnGameWon();
            else
                gameManager.OnGameOver();
        }

        // Disable all spawning and movement
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.SetActive(false);
        }
    }

    /// <summary>
    /// Called when the wave completes successfully
    /// </summary>
    public void OnWaveComplete()
    {
        // Clear all remaining enemies
        Opponent[] remainingEnemies = FindObjectsOfType<Opponent>();
        foreach (var enemy in remainingEnemies)
        {
            Destroy(enemy.gameObject);
        }

        GameOver(true);
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
    /// Get the tower's world position for effects/UI
    /// </summary>
    public Vector3 GetTowerPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Set the canvas for world-space UI elements
    /// </summary>
    public void SetWorldCanvas(Canvas canvas)
    {
        worldCanvas = canvas;
    }

    /// <summary>
    /// Get the world canvas
    /// </summary>
    public Canvas GetWorldCanvas()
    {
        return worldCanvas;
    }
}