using UnityEngine;
using System.Collections.Generic;

// ========== BASE CARD CLASS ==========
public abstract class Card : MonoBehaviour
{
    // Base class for both card types
}

// ========== SUMMON CARD COMPONENT ==========
public class SummonCard : Card
{
    public string cardName;
    public Sprite cardArt;
    public GameObject allyPrefab;
    public float manaCost = 1f;

    private TowerManager towerManager;
    private bool canUse = true;

    private void Start()
    {
        towerManager = FindObjectOfType<TowerManager>();
    }

    /// <summary>
    /// Called when card is thrown onto the battlefield
    /// </summary>
    public void ActivateAtPosition(Vector3 worldPosition)
    {
        if (!canUse)
        {
            Debug.LogWarning($"{cardName} is still on cooldown");
            return;
        }

        if (allyPrefab == null)
        {
            Debug.LogError($"No ally prefab assigned to {cardName}");
            return;
        }

        // Instantiate the ally at the thrown position
        GameObject allyInstance = Instantiate(allyPrefab, worldPosition, Quaternion.identity);
        Ally allyComponent = allyInstance.GetComponent<Ally>();

        if (allyComponent != null)
        {
            allyComponent.Initialize(towerManager);
            towerManager.RegisterAlly(allyComponent);
            Debug.Log($"Summoned {cardName} at position {worldPosition}");
        }
        else
        {
            Debug.LogError($"Ally prefab for {cardName} is missing Ally component");
            Destroy(allyInstance);
        }
    }

    /// <summary>
    /// Enables/disables card use (for cooldowns if needed)
    /// </summary>
    public void SetUsable(bool usable)
    {
        canUse = usable;
    }

    public bool IsUsable()
    {
        return canUse;
    }
}

// ========== SPELL CARD COMPONENT ==========
public class SpellCard : Card
{
    public enum SpellType { AOE, Buff, Heal, Special }

    public string cardName;
    public Sprite cardArt;
    public SpellType spellType;
    public float potency = 10f; // Damage, heal amount, buff multiplier, etc
    public float radius = 5f; // For AOE spells
    public float duration = 5f; // For buff spells
    public float manaCost = 1f;

    private TowerManager towerManager;
    private CardManager cardManager;
    private bool canUse = true;

    private void Start()
    {
        towerManager = FindObjectOfType<TowerManager>();
        cardManager = FindObjectOfType<CardManager>();
    }

    /// <summary>
    /// Called when card is thrown onto the battlefield
    /// </summary>
    public void ActivateAtPosition(Vector3 worldPosition)
    {
        if (!canUse)
        {
            Debug.LogWarning($"{cardName} is still on cooldown");
            return;
        }

        switch (spellType)
        {
            case SpellType.AOE:
                ActivateAOE(worldPosition);
                break;
            case SpellType.Buff:
                ActivateBuff(worldPosition);
                break;
            case SpellType.Heal:
                ActivateHeal(worldPosition);
                break;
            case SpellType.Special:
                ActivateSpecial(worldPosition);
                break;
        }
    }

    private void ActivateAOE(Vector3 targetPosition)
    {
        // Lightning Bolt - damage all enemies in radius
        Debug.Log($"{cardName} activated at {targetPosition}");

        Collider[] hits = Physics.OverlapSphere(targetPosition, radius);
        int enemiesHit = 0;

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<Opponent>(out var opponent))
            {
                opponent.TakeDamage(potency);
                enemiesHit++;
            }
        }

        Debug.Log($"{cardName} hit {enemiesHit} enemies for {potency} damage");

        // Visual effect - you'd instantiate a particle system here
        CreateVisualEffect(targetPosition, "Lightning");
    }

    private void ActivateBuff(Vector3 targetPosition)
    {
        // Athena's Wrath - double attack of all current allies
        Ally[] allAllies = FindObjectsOfType<Ally>();

        foreach (var ally in allAllies)
        {
            ally.ApplyAttackBuff(2f, duration);
        }

        Debug.Log($"{cardName} buffed {allAllies.Length} allies with 2x damage for {duration}s");
        CreateVisualEffect(targetPosition, "Buff");
    }

    private void ActivateHeal(Vector3 targetPosition)
    {
        // Aphrodite's Favor - heal tower or random allies
        if (towerManager.GetHealthPercentage() < 1f)
        {
            // Tower not at full health, heal it
            float healed = towerManager.Heal(potency);
            Debug.Log($"{cardName} healed tower for {healed}");
        }
        else
        {
            // Tower at full health, heal two random allies
            Ally[] allAllies = FindObjectsOfType<Ally>();

            if (allAllies.Length > 0)
            {
                int healsApplied = 0;
                for (int i = 0; i < 2 && allAllies.Length > 0; i++)
                {
                    Ally randomAlly = allAllies[Random.Range(0, allAllies.Length)];
                    randomAlly.Heal(potency);
                    healsApplied++;
                }

                Debug.Log($"{cardName} healed {healsApplied} allies for {potency} each");
            }
        }

        CreateVisualEffect(targetPosition, "Heal");
    }

    private void ActivateSpecial(Vector3 targetPosition)
    {
        // Pandora's Box - 50/50 chance to either play two random cards or damage tower
        if (Random.value > 0.5f)
        {
            PlayTwoRandomCards();
        }
        else
        {
            DamageTowerForQuarter();
        }
    }

    private void PlayTwoRandomCards()
    {
        if (cardManager == null)
        {
            Debug.LogError("CardManager not found for Pandora's Box");
            return;
        }

        List<Card> availableCards = cardManager.GetAvailableCards(cardName);

        if (availableCards.Count < 2)
        {
            Debug.LogWarning("Pandora's Box: Not enough cards available");
            DamageTowerForQuarter(); // Fallback to damage
            return;
        }

        // Pick two random cards (no duplicates)
        Card card1 = availableCards[Random.Range(0, availableCards.Count)];
        availableCards.Remove(card1);
        Card card2 = availableCards[Random.Range(0, availableCards.Count)];

        Debug.Log($"Pandora's Box: Playing {card1.gameObject.name} and {card2.gameObject.name}");

        // Activate the two cards at random positions near tower
        Vector3 randomPos1 = GetRandomNearbyPosition();
        Vector3 randomPos2 = GetRandomNearbyPosition();

        if (card1.TryGetComponent<SummonCard>(out var summon1))
            summon1.ActivateAtPosition(randomPos1);
        else if (card1.TryGetComponent<SpellCard>(out var spell1))
            spell1.ActivateAtPosition(randomPos1);

        if (card2.TryGetComponent<SummonCard>(out var summon2))
            summon2.ActivateAtPosition(randomPos2);
        else if (card2.TryGetComponent<SpellCard>(out var spell2))
            spell2.ActivateAtPosition(randomPos2);
    }

    private void DamageTowerForQuarter()
    {
        float damage = towerManager.GetMaxHealth() * 0.25f;
        towerManager.TakeDamage(damage);
        Debug.Log($"Pandora's Box: Tower took {damage} damage!");
        CreateVisualEffect(towerManager.transform.position, "Explosion");
    }

    private Vector3 GetRandomNearbyPosition()
    {
        Vector3 towerPos = towerManager.transform.position;
        float randomRadius = Random.Range(2f, 5f);
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        return towerPos + new Vector3(
            Mathf.Cos(randomAngle) * randomRadius,
            0,
            Mathf.Sin(randomAngle) * randomRadius
        );
    }

    private void CreateVisualEffect(Vector3 position, string effectType)
    {
        // Instantiate particle system or visual prefab here
        // For now, just debug visualization
        Debug.Log($"Visual effect '{effectType}' at {position}");
    }

    public void SetUsable(bool usable)
    {
        canUse = usable;
    }

    public bool IsUsable()
    {
        return canUse;
    }
}