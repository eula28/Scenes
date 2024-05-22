using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoCaptureImage : MonoBehaviour
{
    // The name of the scene you want to load
    public string IR1CaptureImage;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(IR1CaptureImage);
    }
}
