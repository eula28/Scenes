using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Android;

public class ScreenCaptureHandler : MonoBehaviour
{
    public Button captureButton;
    private string customDirectory = "Histourical Adventure"; // ANDROID DIRECTORY DCIM INTERNAL STORAGE

    private void Start()
    {
        if (captureButton != null)  
        {
            captureButton.onClick.AddListener(CaptureScreenshot);
        }

        // Request necessary permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }

    private void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        // 1 SEC BEFORE SS
        yield return new WaitForSeconds(1f);

        yield return new WaitForEndOfFrame();

        string directoryPath = GetExternalStoragePath();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string fileName = "Histourical_Adventure_" + System.Guid.NewGuid().ToString().Substring(0, 8) + ".jpg"; // GUID SHORT FILE NAME JPG
        string filePath = Path.Combine(directoryPath, fileName);

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        byte[] imageBytes = screenImage.EncodeToJPG();
        File.WriteAllBytes(filePath, imageBytes);

        Debug.Log("Screenshot saved to: " + filePath);

        // Notify user
        ShowToast("Screenshot saved to Histourical Adventure");

        // REFRESH MEMORY
        Destroy(screenImage);
    }

    private string GetExternalStoragePath()
    {
        string externalStoragePath = "/storage/emulated/0/DCIM/" + customDirectory;
        return externalStoragePath;
    }

    private void ShowToast(string message)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (activity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastClass.GetStatic<int>("LENGTH_SHORT"));
                    toast.Call("show");
                }));
            }
        }
    }
}
