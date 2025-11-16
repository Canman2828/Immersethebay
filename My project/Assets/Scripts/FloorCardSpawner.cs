using UnityEngine;

public class FloorCardSpawner : MonoBehaviour
{
    [Header("Athena card prefab to spawn")]
    public GameObject athenaCardPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // This script is on the FLOOR.
        // 'collision.gameObject' is whatever hit the floor.
        GameObject other = collision.gameObject;

        Debug.Log($"Floor collided with: {other.name}, tag: {other.tag}");

        // Only react to Aphrodite's card
        if (!other.CompareTag("AphroditeCard"))
            return;

        // Where the card actually hit the floor
        Vector3 hitPoint = collision.GetContact(0).point;

        // Use the card's rotation (or Quaternion.identity if you prefer flat)
        Quaternion spawnRot = other.transform.rotation;

        // Spawn Athena's card at the hit point
        if (athenaCardPrefab != null)
        {
            Instantiate(athenaCardPrefab, hitPoint, spawnRot);
        }

        // Destroy Aphrodite's card so it "disappears"
        Destroy(other);
    }
}
