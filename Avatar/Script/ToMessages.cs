using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ToMessages : MonoBehaviour
{
    // The name of the scene you want to load
    public string toMessages;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(toMessages);
    }
}
