using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public GameObject notificationPanel; // Reference to the notification panel
    public TMP_Text notificationText; // Reference to the notification text
    public Button retryButton; // Reference to the retry button
    public Button closeButton; // Reference to the close button

    void Start()
    {
        notificationPanel.SetActive(false); // Initially hide the notification panel
        CheckInternetConnection();
    }

    void CheckInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet connection");
            SceneManager.LoadScene("IR1Offline");
        }
        else
        {
            Debug.Log("Internet connection available");
            SceneManager.LoadScene("Welcome-Screen");
        }
    }

    void NotifyUser()
    {
        notificationPanel.SetActive(true);
        notificationText.text = "No internet connection. Please turn on your internet to use the full feature of the App.";
        retryButton.onClick.AddListener(RetryConnection);
        closeButton.onClick.AddListener(CloseNotificationPanel);
    }

    void RetryConnection()
    {
        notificationPanel.SetActive(false);
        CheckInternetConnection();
    }

    void CloseNotificationPanel()
    {
        notificationPanel.SetActive(false);
    }
}
