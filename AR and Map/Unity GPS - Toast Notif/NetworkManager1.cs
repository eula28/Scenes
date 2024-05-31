using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager1 : MonoBehaviour
{
    void Start()
    {
        CheckInternetConnection1();
    }

    void CheckInternetConnection1()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection");
            // Replace "OfflineScene" with the name of your desired offline screen scene
            SceneManager.LoadScene("IR11");
        }
        else
        {
            Debug.Log("Internet connection available");
            // Continue with the normal flow or load the main scene
            // Replace "MainScene" with the name of your main scene
            SceneManager.LoadScene("Welcome-Screen");
        }
    }
}
