using UnityEngine.SceneManagement;
using UnityEngine;

public class CubeButton : MonoBehaviour
{
    // The name of the scene you want to load
    public string test;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(test);
    }
}
