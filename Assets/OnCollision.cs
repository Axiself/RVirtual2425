using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class MakeInvisibleOnCollision : MonoBehaviour
{
    private Renderer objectRenderer; // For controlling visibility
    private Collider objectCollider; // For disabling collision
    private Canvas canvas;

    void Start()
    {
        File.AppendAllLines(Path.Combine(Application.persistentDataPath, gameObject.name + "animal.csv"), new List<string>() { $"Frame\tAnimalName"});
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();
        GameObject canvasObject = GameObject.FindGameObjectWithTag("Notification");
        canvas = canvasObject.GetComponent<Canvas>();
    }



    void OnCollisionEnter(Collision collision)
    {

        // Make B invisible
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false; // Hides the object
            canvas.enabled = true;
            
        }

        // Remove B's collider
        if (objectCollider != null)
        {
            objectCollider.enabled = false; // Disables collision
        }

        File.AppendAllLines(Path.Combine(Application.persistentDataPath, gameObject.name + "animal.csv"), new List<string>() { $"{Time.frameCount}\t{gameObject.name}" });
    }
}

