using UnityEngine;

public class LocationServicesChecker : MonoBehaviour
{
    void Start()
    {
        if (!IsLocationServiceEnabled())
        {
            PromptEnableLocation();
        }
    }

    bool IsLocationServiceEnabled()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    using (var locationManager = context.Call<AndroidJavaObject>("getSystemService", "location"))
                    {
                        return locationManager.Call<bool>("isProviderEnabled", "gps");
                    }
                }
            }
        }
    }

    void PromptEnableLocation()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                currentActivity.Call("startActivity", new AndroidJavaObject("android.content.Intent", "android.settings.LOCATION_SOURCE_SETTINGS"));
            }
        }
    }
}
