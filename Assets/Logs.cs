using System;
using System.IO;
using System.Text;
using UnityEngine;

public class WritePositionData : MonoBehaviour
{
    //private string positionDataFilePath;
    //[Header("OVRCameraRig")]
    //[SerializeField] private GameObject Head;
    //[SerializeField] private GameObject Inte;
    //private StreamWriter file;
    //private Vector3 previousInteractablePosition;

    //void Start()
    //{
    //    positionDataFilePath = Path.Combine(Application.persistentDataPath, "log.csv");
    //    file = new StreamWriter(new FileStream(positionDataFilePath, FileMode.Create), Encoding.UTF8);
    //    // Write the header line to the position data file
    //    file.WriteLine("TimeStamp,HeadX,HeadY,HeadZ");
    //    previousInteractablePosition = Interactable.transform.position; // Initialize the previous position
    //}

    //void Update()
    //{
    //    // Fetch the current position of the VR headset
    //    Vector3 currentHeadsetPosition = Head.transform.position;
    //    Vector3 currentInteractablePosition = Interactable.transform.position;

    //    // Check if the position has changed
    //    if (currentInteractablePosition != previousInteractablePosition)
    //    {
    //        // Format the current positions into a string for the CSV file
    //        string positionDataLine = string.Format(
    //            "{0},{1},{2},{3}",
    //            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), // Use a safe timestamp format
    //            currentHeadsetPosition.x.ToString("F6"), // Format position values with 6 decimal places
    //            currentHeadsetPosition.y.ToString("F6"),
    //            currentHeadsetPosition.z.ToString("F6")
    //        );

    //        // Write the position data line to the CSV file
    //        file.WriteLine(positionDataLine);

    //        // Update the previous position to the current position
    //        previousInteractablePosition = currentInteractablePosition;
    //    }
    //}

    //void OnApplicationQuit()
    //{
    //    file.Flush();
    //    file.Close();
    //}
}
