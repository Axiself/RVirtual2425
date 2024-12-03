using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

public class WritePositionData : MonoBehaviour
{
    private string positionDataFilePath;
    public GameObject Head;
    private StreamWriter file;
    void Start()
    {
        positionDataFilePath = Path.Combine(Application.persistentDataPath, "log.csv");
        file = new StreamWriter(new FileStream(positionDataFilePath, FileMode.Create), Encoding.UTF8);
        // Write the header line to the position data file
        file.WriteLine("TimeStamp,HeadX,HeadY,HeadZ");
    }
    void Update()
    {
        // Fetch the positions of the VR headset
        Vector3 currentHeadsetPosition = Head.transform.position;
        // Format the current positions into a string for the CSV file
        string positionDataLine = string.Format(
            "{0},{1},{2},{3}",
            DateTime.Now,
            currentHeadsetPosition.x, currentHeadsetPosition.y, currentHeadsetPosition.z
        );
        file.WriteLine(positionDataLine);
    }
    void OnApplicationQuit()
    {
        file.Flush();
        file.Close();
    }
}
