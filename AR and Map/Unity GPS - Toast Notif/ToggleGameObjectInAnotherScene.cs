using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToggleGameObjectInAnotherScene : MonoBehaviour
{
    public string targetSceneName; // Name of the target scene
    public string targetGameObjectName; // Name of the target GameObject in the target scene
    public Button button; // Assign the button in the Unity Editor
    public float delayInMinutes = 40f; // Delay time in minutes

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnButtonClick()
    {
        StartCoroutine(DisableAndReactivateGameObject());
    }

    private IEnumerator DisableAndReactivateGameObject()
    {
        // Load the target scene additively
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Find the target GameObject in the loaded scene
        GameObject targetGameObject = GameObject.Find(targetGameObjectName);

        if (targetGameObject != null)
        {
            // Disable the target GameObject
            targetGameObject.SetActive(false);

            // Start coroutine to reactivate the GameObject after the delay
            StartCoroutine(ReactivateGameObjectAfterDelay(targetGameObject, delayInMinutes * 60)); // Convert minutes to seconds
        }
        else
        {
            Debug.LogError("Target GameObject not found in the target scene.");
        }
    }

    private IEnumerator ReactivateGameObjectAfterDelay(GameObject targetGameObject, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (targetGameObject != null)
        {
            targetGameObject.SetActive(true);
        }

        // Optionally, unload the target scene if it's no longer needed
        SceneManager.UnloadSceneAsync(targetSceneName);
    }
}
