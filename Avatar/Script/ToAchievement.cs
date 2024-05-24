using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ToAchievement : MonoBehaviour
{
    // The name of the scene you want to load
    public string toAchievement;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(toAchievement);
    }
}
