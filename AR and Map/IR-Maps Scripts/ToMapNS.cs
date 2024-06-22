using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtontoMapNS : MonoBehaviour
{
    // The name of the scene you want to load
    public string MapNS;

    public void OnMouseUpAsButton()
    {
        SceneManager.LoadScene(MapNS);
    }
}
