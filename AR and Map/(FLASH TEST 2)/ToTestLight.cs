using UnityEngine.SceneManagement;
using UnityEngine;

public class ToTestLight : MonoBehaviour
{
    // The name of the scene you want to load
    public string toTestlight;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(toTestlight);
    }
}
