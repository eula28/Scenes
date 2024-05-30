using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoLocationInstruction : MonoBehaviour
{
    // The name of the scene you want to load
    public string LocationInstruction;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(LocationInstruction);
    }
}
