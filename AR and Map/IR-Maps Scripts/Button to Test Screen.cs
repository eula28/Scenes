using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CubeButton : MonoBehaviour
{
    // The name of the scene you want to load
    public string test;

    private void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(test);
    }
}
