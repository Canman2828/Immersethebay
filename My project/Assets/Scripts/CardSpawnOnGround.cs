using UnityEngine;

public class CardSpawnOnGround : MonoBehaviour
{
    [Header("Optional: card or effect prefab to spawn on impact")]
    public GameObject cardOrEffectPrefab;

    [Header("Floor tag to react to")]
    public string groundTag = "Ground";

    [Header("What to do after triggering")]
    public bool destroyCardAfterTrigger = true;

    private bool hasTriggered = false;

    // Half-inch offset in meters
    private const float HALF_INCH = 0.0127f;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasTriggered)
            return;

        if (!collision.gameObject.CompareTag(groundTag))
            return;

        hasTriggered = true;

        // Exact hit point
        Vector3 hitPoint = collision.GetContact(0).point;

        // Apply the upward offset
        Vector3 spawnPoint = new Vector3(
            hitPoint.x,
            hitPoint.y + HALF_INCH,
            hitPoint.z
        );

        //
        // OPTION A: This card itself has the SpellCard/SummonCard
        //
        if (TryGetComponent<SpellCard>(out var spellOnThisObject))
        {
            spellOnThisObject.ActivateAtPosition(spawnPoint);
        }

        if (TryGetComponent<SummonCard>(out var summonOnThisObject))
        {
            summonOnThisObject.ActivateAtPosition(spawnPoint);
        }

        //
        // OPTION B: Spawn prefab + activate it
        //
        if (cardOrEffectPrefab != null)
        {
            GameObject spawned = Instantiate(cardOrEffectPrefab, spawnPoint, transform.rotation);

            if (spawned.TryGetComponent<SpellCard>(out var spawnedSpell))
                spawnedSpell.ActivateAtPosition(spawnPoint);

            if (spawned.TryGetComponent<SummonCard>(out var spawnedSummon))
                spawnedSummon.ActivateAtPosition(spawnPoint);
        }

        if (destroyCardAfterTrigger)
        {
            Destroy(gameObject);
        }
    }
}
