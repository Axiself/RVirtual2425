using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToPracticeScene : MonoBehaviour
{
    public void GoToPracticeScene()
    {
        SceneManager.LoadScene("Practice");
    }
}
