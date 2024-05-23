using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    private AndroidJavaObject cameraManager;
    private string cameraId;
    private bool isFlashlightOn = false;

    void Start()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        // Get the camera service
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.camera2.CameraManager");
        cameraManager = context.Call<AndroidJavaObject>("getSystemService", "camera");

        cameraId = cameraManager.Call<string[]>("getCameraIdList")[0];
        #endif
    }

    public void ToggleFlashlight()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (cameraManager != null)
        {
            isFlashlightOn = !isFlashlightOn;
            cameraManager.Call("setTorchMode", cameraId, isFlashlightOn);
        }
        #endif
    }
}
