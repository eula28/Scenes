using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoMap : MonoBehaviour
{
    // The name of the scene you want to load
    public string Map;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(Map);
    }
}
