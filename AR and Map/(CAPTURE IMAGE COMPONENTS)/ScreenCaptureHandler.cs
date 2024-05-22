using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenCaptureHandler : MonoBehaviour
{
    public Button captureButton;
    private string customDirectory = "HistouricalAdventure"; // Custom directory to save screenshots

    private void Start()
    {
        if (captureButton != null)  
        {
            captureButton.onClick.AddListener(CaptureScreenshot);
        }
    }

    private void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        // Wait for 1 second before capturing the screenshot
        yield return new WaitForSeconds(1f);

        yield return new WaitForEndOfFrame();

        string directoryPath = GetExternalStoragePath();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string fileName = "HistouricalAdventure_" + System.Guid.NewGuid().ToString() + ".jpg";
        string filePath = Path.Combine(directoryPath, fileName);

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        byte[] imageBytes = screenImage.EncodeToJPG();
        File.WriteAllBytes(filePath, imageBytes);

        Debug.Log("Screenshot saved to: " + filePath);

        // Clean up the texture to free memory
        Destroy(screenImage);
    }

    private string GetExternalStoragePath()
    {
        string externalStoragePath = "/storage/emulated/0/DCIM/" + customDirectory;
        return externalStoragePath;
    }
}
