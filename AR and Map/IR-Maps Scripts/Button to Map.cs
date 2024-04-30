using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoMap : MonoBehaviour
{
    // The name of the scene you want to load
    public string Map;

    private void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(Map);
    }
}
