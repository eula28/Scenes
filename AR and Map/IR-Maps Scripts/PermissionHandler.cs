using UnityEngine;
using UnityEngine.Android;

public class PermissionHandler : MonoBehaviour
{
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string[] permissions = {
                "android.permission.INTERNET",
                "android.permission.ACCESS_NETWORK_STATE",
                "android.permission.ACCESS_FINE_LOCATION",
                "android.permission.ACCESS_COARSE_LOCATION"
            };

            foreach (string permission in permissions)
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    Permission.RequestUserPermission(permission);
                }
            }
        }
    }
}
