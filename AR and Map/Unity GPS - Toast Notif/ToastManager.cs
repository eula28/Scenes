using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static void ShowToast(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity != null)
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                    {
                        using (AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast"))
                        {
                            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastClass.GetStatic<int>("LENGTH_SHORT"));
                            toast.Call("show");
                        }
                    }));
                }
            }
        }
        else
        {
            Debug.Log("Toast: " + message); // Fallback for other platforms
        }
    }
};
