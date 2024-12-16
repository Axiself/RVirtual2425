using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToHeadControl3rdPersonScene : MonoBehaviour
{
    public void GoToHeadControl3rdPersonScene()
    {
        SceneManager.LoadScene("Head Control 3rd Person");
    }
}
