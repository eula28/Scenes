using UnityEngine.SceneManagement;
using UnityEngine;

public class ToAccount : MonoBehaviour
{
    // The name of the scene you want to load
    public string toAccount;

    public void OnMouseUpAsButton()
    {
        // Load the specified scene
        SceneManager.LoadScene(toAccount);
    }
}
