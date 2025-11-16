using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.ComponentModel;
using UnityEditor;

public class Ally : MonoBehaviour
{
    [SerializeField] private UnitStats stats;
    public Image healthFillImage;

    private TowerManager towerManager;
    private Opponent currentTarget;
    private float inactivityTimer;
    private const float INACTIVITY_TIMEOUT = 20f;
    private bool isActive = true;
    private bool hasBeenEngaged = false;

    // Attack timing
    private float timeSinceLastAttack = 0f;

    /// <summary>
    /// Initialize the ally with tower manager reference
    /// </summary>
    public void Initialize(TowerManager tower)
    {
        towerManager = tower;
        inactivityTimer = 0f;
        timeSinceLastAttack = 0f;
        hasBeenEngaged = false;

        // Set up health fill image if attached
        if (healthFillImage == null)
        {
            healthFillImage = GetComponentInChildren<Image>();
        }

        UpdateHealthDisplay();
    }

    /// <summary>
    /// Set the unit stats for this ally (called by summon manager or inspector)
    /// </summary>
    public void SetStats(UnitStats newStats)
    {
        stats = new UnitStats(
            newStats.maxHealth,
            newStats.damage,
            newStats.attackRate,
            newStats.speed,
            0 // healthBarTicks not used
        );
        stats.health = stats.maxHealth;
        UpdateHealthDisplay();
    }

    private void Update()
    {
        if (!isActive) return;

        // Update attack cooldown
        if (timeSinceLastAttack < 1f / stats.attackRate)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        // Handle target engagement
        if (currentTarget != null && currentTarget.gameObject.activeSelf)
        {
            hasBeenEngaged = true;
            inactivityTimer = 0f; // Reset inactivity timer while engaged

            // Attack the target
            if (timeSinceLastAttack >= 1f / stats.attackRate)
            {
                Attack(currentTarget);
                timeSinceLastAttack = 0f;
            }
        }
        else
        {
            // No target, increment inactivity timer
            inactivityTimer += Time.deltaTime;
            currentTarget = null;

            // Despawn if inactive for too long
            if (hasBeenEngaged && inactivityTimer >= INACTIVITY_TIMEOUT)
            {
                Despawn();
            }
        }
    }

    /// <summary>
    /// Attack the target opponent
    /// </summary>
    private void Attack(Opponent target)
    {
        if (target == null) return;

        target.TakeDamage(stats.damage);
        Debug.Log($"{gameObject.name} attacks for {stats.damage} damage!");

        // You could add attack animation or sound here
    }

    /// <summary>
    /// Receive damage from opponent
    /// </summary>
    public void TakeDamage(float damage)
    {
        stats.health -= damage;
        stats.health = Mathf.Max(stats.health, 0);

        UpdateHealthDisplay();

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {stats.health}/{stats.maxHealth}");

        if (stats.health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heal this ally
    /// </summary>
    public void Heal(float amount)
    {
        float oldHealth = stats.health;
        stats.health = Mathf.Min(stats.health + amount, stats.maxHealth);

        float actualHealed = stats.health - oldHealth;
        Debug.Log($"{gameObject.name} healed for {actualHealed}. Health: {stats.health}/{stats.maxHealth}");

        UpdateHealthDisplay();
    }

    /// <summary>
    /// Apply a damage buff for a duration
    /// </summary>
    public void ApplyAttackBuff(float multiplier, float duration)
    {
        StartCoroutine(BuffAttackCoroutine(multiplier, duration));
    }

    private IEnumerator BuffAttackCoroutine(float multiplier, float duration)
    {
        float originalDamage = stats.damage;
        stats.damage *= multiplier;

        Debug.Log($"{gameObject.name} buffed! Damage: {originalDamage} -> {stats.damage}");

        yield return new WaitForSeconds(duration);

        stats.damage = originalDamage;
        Debug.Log($"{gameObject.name} buff expired. Damage: {stats.damage}");
    }

    /// <summary>
    /// Called when the ally dies
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        isActive = false;

        if (towerManager != null)
        {
            towerManager.UnregisterAlly(this);
        }

        // You could add death animation or particle effect here
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the ally is despawned due to inactivity
    /// </summary>
    private void Despawn()
    {
        Debug.Log($"{gameObject.name} despawned due to inactivity!");
        isActive = false;

        if (towerManager != null)
        {
            towerManager.UnregisterAlly(this);
        }

        // You could add despawn animation or particle effect here
        Destroy(gameObject);
    }

    /// <summary>
    /// Update the health fill image to show current health percentage
    /// </summary>
    private void UpdateHealthDisplay()
    {
        if (healthFillImage != null)
        {
            float healthPercent = stats.health / stats.maxHealth;
            healthFillImage.fillAmount = healthPercent;
        }
    }

    /// <summary>
    /// Get current health percentage (0 to 1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return stats.health / stats.maxHealth;
    }

    // ========== COLLISION HANDLING ==========

    private void OnTriggerEnter(Collider collision)
    {
        // Detect when an opponent enters the ally's collider
        if (collision.TryGetComponent<Opponent>(out var opponent))
        {
            if (currentTarget == null)
            {
                currentTarget = opponent;
                Debug.Log($"{gameObject.name} engaged target: {opponent.gameObject.name}");
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        // Continue attacking current target if they're still in collider
        if (currentTarget != null && collision.gameObject == currentTarget.gameObject)
        {
            // This is handled in Update(), but we can use this to verify target is still present
            if (!currentTarget.gameObject.activeSelf)
            {
                currentTarget = null;
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        // Opponent left the collider
        if (currentTarget != null && collision.gameObject == currentTarget.gameObject)
        {
            Debug.Log($"{gameObject.name} lost target: {currentTarget.gameObject.name}");
            currentTarget = null;
        }
    }

    /// <summary>
    /// Get the current stats of this ally
    /// </summary>
    public UnitStats GetStats()
    {
        return stats;
    }

    /// <summary>
    /// Check if ally is still active
    /// </summary>
    public bool IsActive()
    {
        return isActive;
    }
}