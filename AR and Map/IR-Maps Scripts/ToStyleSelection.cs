using UnityEngine.SceneManagement;
using UnityEngine;

public class StyleSelection : MonoBehaviour
{
    // The name of the scene you want to load
    public string styleSelection;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(styleSelection);
    }
}
