using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Opponent : MonoBehaviour
{
    [SerializeField] protected UnitStats stats;
    [SerializeField] protected Image healthFillImage;

    protected List<Vector3> pathPoints = new List<Vector3>();
    protected int currentPathIndex = 0;
    protected Rigidbody rb;
    protected Ally currentTarget;

    protected float timeSinceLastAttack = 0f;
    private bool isAlive = true;
    private TowerManager towerManager;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        towerManager = FindObjectOfType<TowerManager>();
        timeSinceLastAttack = 0f;

        // Set up health fill image if attached
        if (healthFillImage == null)
        {
            healthFillImage = GetComponentInChildren<Image>();
        }

        UpdateHealthDisplay();
    }

    protected virtual void Update()
    {
        if (!isAlive) return;

        // Update attack cooldown
        if (timeSinceLastAttack < 1f / stats.attackRate)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        // Handle target engagement
        if (currentTarget != null && currentTarget.IsActive())
        {
            // Attack current target
            if (timeSinceLastAttack >= 1f / stats.attackRate)
            {
                Attack(currentTarget);
                timeSinceLastAttack = 0f;
            }
        }
        else
        {
            // No target, follow path
            currentTarget = null;
            FollowPath();
        }
    }

    /// <summary>
    /// Set the path this opponent should follow
    /// </summary>
    public void SetPath(List<Vector3> newPath)
    {
        pathPoints = new List<Vector3>(newPath);
        currentPathIndex = 0;
    }

    /// <summary>
    /// Follow the predefined path
    /// </summary>
    protected virtual void FollowPath()
    {
        if (pathPoints.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no path points!");
            return;
        }

        if (currentPathIndex >= pathPoints.Count)
        {
            // Reached the end of the path - attack tower
            AttackTower();
            return;
        }

        Vector3 targetPoint = pathPoints[currentPathIndex];
        float distance = Vector3.Distance(transform.position, targetPoint);

        // Move toward the target point
        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.velocity = direction * stats.speed;

        // Check if reached waypoint
        if (distance < 0.5f)
        {
            currentPathIndex++;
        }
    }

    /// <summary>
    /// Attack a target ally
    /// </summary>
    protected virtual void Attack(Ally target)
    {
        if (target == null || !target.IsActive()) return;

        target.TakeDamage(stats.damage);
        Debug.Log($"{gameObject.name} attacks {target.gameObject.name} for {stats.damage} damage!");
    }

    /// <summary>
    /// Attack the tower when reaching the end of path
    /// </summary>
    private void AttackTower()
    {
        if (towerManager == null) return;

        if (timeSinceLastAttack >= 1f / stats.attackRate)
        {
            towerManager.TakeDamage(stats.damage);
            Debug.Log($"{gameObject.name} attacks tower for {stats.damage} damage!");
            timeSinceLastAttack = 0f;
        }
    }

    /// <summary>
    /// Receive damage
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (!isAlive) return;

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
    /// Die and disappear
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        isAlive = false;
        rb.velocity = Vector3.zero;

        // You could add death animation or particle effect here
        Destroy(gameObject);
    }

    /// <summary>
    /// Update the health fill image to show current health percentage
    /// </summary>
    protected void UpdateHealthDisplay()
    {
        if (healthFillImage != null)
        {
            float healthPercent = stats.health / stats.maxHealth;
            healthFillImage.fillAmount = healthPercent;
        }
    }

    /// <summary>
    /// Set unit stats
    /// </summary>
    public void SetStats(UnitStats newStats)
    {
        stats = new UnitStats(
            newStats.maxHealth,
            newStats.damage,
            newStats.attackRate,
            newStats.speed,
            0
        );
        stats.health = stats.maxHealth;
        UpdateHealthDisplay();
    }

    /// <summary>
    /// Get current health percentage
    /// </summary>
    public float GetHealthPercentage()
    {
        return stats.health / stats.maxHealth;
    }

    /// <summary>
    /// Check if opponent is alive
    /// </summary>
    public bool IsAlive()
    {
        return isAlive;
    }

    // ========== COLLISION HANDLING ==========

    private void OnTriggerEnter(Collider collision)
    {
        if (!isAlive) return;

        // Detect collision with ally
        if (collision.TryGetComponent<Ally>(out var ally))
        {
            if (currentTarget == null)
            {
                currentTarget = ally;
                Debug.Log($"{gameObject.name} engaged ally: {ally.gameObject.name}");
                rb.velocity = Vector3.zero; // Stop movement
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (!isAlive) return;

        // Continue engaging ally if they're still in collider
        if (currentTarget != null && collision.gameObject == currentTarget.gameObject)
        {
            rb.velocity = Vector3.zero; // Stay in place while fighting
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!isAlive) return;

        // Ally left the collider
        if (currentTarget != null && collision.gameObject == currentTarget.gameObject)
        {
            Debug.Log($"{gameObject.name} disengaged from {currentTarget.gameObject.name}");
            currentTarget = null;
        }
    }
}

// ========== SPECIFIC OPPONENT TYPES ==========

public class Werewolf : Opponent
{
    // Werewolf: 5 health, 3 damage, 1 attack/sec, 3 speed (fast), 1 tick
    protected override void Start()
    {
        stats = new UnitStats(
            maxHealthValue: 5f,
            damageValue: 3f,
            attackRateValue: 1f,
            speedValue: 3f,
            ticksValue: 1
        );
        base.Start();
    }
}

public class CerberusOpponent : Opponent
{
    // Cerberus: 80 health, 39 damage, 1 attack/2.5 sec, 1 speed (slow), 16 ticks
    protected override void Start()
    {
        stats = new UnitStats(
            maxHealthValue: 80f,
            damageValue: 39f,
            attackRateValue: 1f / 2.5f,
            speedValue: 1f,
            ticksValue: 16
        );
        base.Start();
    }
}

public class Harpy : Opponent
{
    // Harpy: 20 health, 10 damage, 1 attack/2 sec, 2 speed (medium), 4 ticks
    protected override void Start()
    {
        stats = new UnitStats(
            maxHealthValue: 20f,
            damageValue: 10f,
            attackRateValue: 1f / 2f,
            speedValue: 2f,
            ticksValue: 4
        );
        base.Start();
    }
}