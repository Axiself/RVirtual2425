using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToHandControlScene : MonoBehaviour
{
    public void GoToHandControlScene()
    {
        SceneManager.LoadScene("Hand Control");
    }
}
