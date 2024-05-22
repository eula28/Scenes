using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoFlashTestScreen : MonoBehaviour
{
    // The name of the scene you want to load
    public string IRTestFlashlight;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(IRTestFlashlight);
    }
}
