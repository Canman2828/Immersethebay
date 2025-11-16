using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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