using UnityEngine;

public class CardCollisionTester : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name} hit {collision.gameObject.name} (tag: {collision.gameObject.tag})");
    }
}
