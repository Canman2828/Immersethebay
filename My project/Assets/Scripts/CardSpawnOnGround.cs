using UnityEngine;

public class CardSpawnOnGround : MonoBehaviour
{
    [Header("Card to spawn when we hit the floor")]
    public GameObject athenaCardPrefab;

    [Header("Floor tag to react to")]
    public string groundTag = "Ground";

    private bool hasSpawned = false;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name} hit {collision.gameObject.name} (tag: {collision.gameObject.tag})");

        if (hasSpawned)
            return;

        if (!collision.gameObject.CompareTag(groundTag))
            return;

        hasSpawned = true;

        // Exact hit point on the floor
        Vector3 hitPoint = collision.GetContact(0).point;

        // You can tweak rotation if needed; for now, use the card's rotation
        Quaternion spawnRot = transform.rotation;

        if (athenaCardPrefab != null)
        {
            Instantiate(athenaCardPrefab, hitPoint, spawnRot);
        }

        // Destroy Aphrodite card
        Destroy(gameObject);
    }
}
