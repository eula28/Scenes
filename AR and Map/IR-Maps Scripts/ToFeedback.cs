using UnityEngine.SceneManagement;
using UnityEngine;

public class ToFeedback : MonoBehaviour
{
    // The name of the scene you want to load
    public string location;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene("Feedback");
        PlayerPrefs.SetString("location", location);
    }
}
