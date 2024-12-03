using UnityEngine;

public class MakeInvisibleOnCollision : MonoBehaviour
{
    private Renderer objectRenderer; // For controlling visibility
    private Collider objectCollider; // For disabling collision

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Make B invisible
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false; // Hides the object
        }

        // Remove B's collider
        if (objectCollider != null)
        {
            objectCollider.enabled = false; // Disables collision
        }
    }
}

