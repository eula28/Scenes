using UnityEngine;
using System.Collections;
using UnityEngine.Android;

public class ToastNotification : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to show the toast after 3 seconds
        StartCoroutine(ShowToastAfterDelay(1));
    }

    IEnumerator ShowToastAfterDelay(float delay)
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delay);

        // Check if the platform is Android
        if (Application.platform == RuntimePlatform.Android)
        {
            // Show the toast notification
            ShowAndroidToastMessage("You don't have an internet connection, Going Offline mode");
        }
    }

    void ShowAndroidToastMessage(string message)
    {
        // Create a new AndroidJavaClass instance for UnityPlayer
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        // Get the currentActivity from the UnityPlayer class
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        // Create a new AndroidJavaObject for Toast
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", message);
        AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString, toastClass.GetStatic<int>("LENGTH_LONG"));
        toastObject.Call("show");
    }
}
