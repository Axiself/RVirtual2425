using UnityEngine;
using UnityEngine.UI; // Required for working with UI elements
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class MakeInvisibleOnCollisionHandG2 : MonoBehaviour
{
    private Renderer objectRenderer; // For controlling visibility
    private Collider objectCollider; // For disabling collision
    private TextMeshPro canvasText; // Text component to update the x/5 value
    public static int count = 0;
    public TextMeshProUGUI text;

    void Start()
    {
        // Initialize the CSV file
        File.AppendAllLines(Path.Combine(Application.persistentDataPath, gameObject.name + "animal.csv"), new List<string>() { $"Frame\tAnimalName" });

        // Get object components
        objectRenderer = GetComponent<Renderer>();
        objectCollider = GetComponent<Collider>();

        // Find the canvas and its text component
        GameObject canvasObject = GameObject.FindGameObjectWithTag("Notification");
        if (canvasObject != null)
        {
            text = canvasObject.GetComponentInChildren<TextMeshProUGUI>(); // Assumes the Text is a child of the canvas
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Make the object invisible
        if (objectRenderer != null)
        {
            objectRenderer.enabled = false; // Hides the object
        }

        // Disable the collider
        if (objectCollider != null)
        {
            objectCollider.enabled = false; // Disables collision
        }
        
        // Increment count and update canvas text
        count++;
        Debug.LogError("Canvas text: " + canvasText);
        Debug.LogError("Count: " + count);
        text.text = count + "/5"; // Format the text as x/5

        // Log the collision to the CSV file
        File.AppendAllLines(Path.Combine(Application.persistentDataPath, gameObject.name + "animal.csv"), new List<string>() { $"{Time.frameCount}\t{gameObject.name}" });
        
        if (count == 5)
        {
            count = 0;
            SceneManager.LoadScene("Post-task 1 (Group 2)");
        }
        
    }
}




