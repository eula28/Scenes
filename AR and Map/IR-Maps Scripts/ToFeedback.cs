using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ToFeedback : MonoBehaviour
{
    // The name of the scene you want to load
    public string Feedback;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene("Feedback");
    }
}
