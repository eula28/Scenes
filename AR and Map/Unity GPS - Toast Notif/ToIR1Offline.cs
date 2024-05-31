using UnityEngine.SceneManagement;
using UnityEngine;

public class ToIR1Offline : MonoBehaviour
{
    // The name of the scene you want to load
    public string IR1Offline;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene("IR1Offline");
    }
}
