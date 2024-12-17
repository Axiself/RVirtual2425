using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToHandControlG2Scene : MonoBehaviour
{
    public void GoToHandControlg2Scene()
    {
        SceneManager.LoadScene("Hand Control 3rd Person");
    }
}
