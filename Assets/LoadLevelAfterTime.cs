using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class LoadLevelAfterTime : MonoBehaviour
{
    [SerializeField]
    private float delayBeforeLoad = 450f;
    [SerializeField]
    private string sceneNameToLoad;
    public static float timeElapsed;

    [SerializeField]
    public string groupN;
    [SerializeField]
    public string controlType;

    void Awake()
    {
        // Initialize the CSV file
        File.AppendAllLines(Path.Combine(Application.persistentDataPath, groupN + controlType + ".csv"), new List<string>() { $"Animal Name,Time Found" });
    }

    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > delayBeforeLoad)
        {
            timeElapsed = 0.0f;
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}
