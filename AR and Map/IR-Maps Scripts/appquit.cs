using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class appquit : MonoBehaviour
{

    // Method to quit the application
    public void QuitApp()
    {
        // If running in the Unity editor, stop playing the scene
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running in a standalone build, quit the application
        Application.Quit();
        #endif
    }
}
