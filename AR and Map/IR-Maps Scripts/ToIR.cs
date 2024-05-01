using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoIR : MonoBehaviour
{
    // The name of the scene you want to load
    public string IR1;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(IR1);
    }
}
