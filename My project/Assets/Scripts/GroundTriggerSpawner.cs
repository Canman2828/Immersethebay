using UnityEngine;

public class GroundTriggerSpawner : MonoBehaviour
{
    [Header("Card to spawn when Aphrodite hits the ground")]
    public GameObject athenaCardPrefab;

    [Header("What counts as 'ground' for snapping")]
    public LayerMask groundLayers;   // usually Default

    private void OnTriggerEnter(Collider other)
    {
        // Only react to the Aphrodite card
        if (!other.CompareTag("AphroditeCard"))
            return;

        // Start with the card's position
        Vector3 spawnPos = other.transform.position;
        Quaternion spawnRot = other.transform.rotation;

        // Optional: Raycast down to find the floor so the new card sits flush
        if (Physics.Raycast(spawnPos + Vector3.up, Vector3.down,
                            out RaycastHit hit, 5f, groundLayers))
        {
            spawnPos = hit.point;
        }

        // Spawn Athena's card
        if (athenaCardPrefab != null)
        {
            Instantiate(athenaCardPrefab, spawnPos, spawnRot);
        }

        // Destroy the Aphrodite card
        Destroy(other.gameObject);
    }
}
