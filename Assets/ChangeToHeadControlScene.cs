using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToHeadControlScene : MonoBehaviour
{
    public void GoToHeadControlScene()
    {
        SceneManager.LoadScene("Hand Control");
    }
}
