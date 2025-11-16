using UnityEngine;

/// <summary>
/// Data structure holding all stats for any unit type (Tower, Ally, Opponent)
/// Used across all game systems for consistent stat management
/// </summary>
[System.Serializable]
public class UnitStats
{
    /// <summary>Current health points (0 to maxHealth)</summary>
    public float health;

    /// <summary>Maximum health points (never changes)</summary>
    public float maxHealth;

    /// <summary>Damage dealt per attack</summary>
    public float damage;

    /// <summary>Attack rate in attacks per second (e.g., 1.0 = 1 attack/sec, 0.5 = 1 attack/2 sec)</summary>
    public float attackRate;

    /// <summary>Movement speed (units per second)</summary>
    public float speed;

    /// <summary>Not used in this version (replaced with filled image health bar)</summary>
    public int healthBarTicks;

    /// <summary>
    /// Create a new UnitStats instance with specified values
    /// </summary>
    public UnitStats(float maxHealthValue, float damageValue, float attackRateValue, float speedValue, int ticksValue)
    {
        maxHealth = maxHealthValue;
        health = maxHealthValue;
        damage = damageValue;
        attackRate = attackRateValue;
        speed = speedValue;
        healthBarTicks = ticksValue;
    }

    /// <summary>
    /// Create a copy of these stats (for creating new unit instances)
    /// </summary>
    public UnitStats Copy()
    {
        return new UnitStats(maxHealth, damage, attackRate, speed, healthBarTicks);
    }

    /// <summary>
    /// Reset health to maximum
    /// </summary>
    public void ResetHealth()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Get health as percentage (0 to 1)
    /// </summary>
    public float GetHealthPercentage()
    {
        return health / maxHealth;
    }

    /// <summary>
    /// Check if unit is at full health
    /// </summary>
    public bool IsFullHealth()
    {
        return health >= maxHealth;
    }

    /// <summary>
    /// Check if unit is dead
    /// </summary>
    public bool IsDead()
    {
        return health <= 0;
    }

    /// <summary>
    /// Apply damage (clamped to 0)
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Max(health, 0);
    }

    /// <summary>
    /// Apply healing (clamped to maxHealth)
    /// </summary>
    public void Heal(float healAmount)
    {
        health += healAmount;
        health = Mathf.Min(health, maxHealth);
    }
}