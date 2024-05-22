using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChangeButton : MonoBehaviour
{
    public string sceneName; // Name of the scene you want to load

    private void Start()
    {
        // Get the button component and add a listener to the onClick event
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        // Start the coroutine to change the scene after 3 seconds
        StartCoroutine(ChangeSceneAfterDelay(3f));
    }

    private IEnumerator ChangeSceneAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}
