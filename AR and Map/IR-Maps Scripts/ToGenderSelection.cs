using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonGenderSelection : MonoBehaviour
{
    // The name of the scene you want to load
    public string GenderSelection;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(GenderSelection);
    }
}
