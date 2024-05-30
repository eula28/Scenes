using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmailVerificationCheck : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseUser user;
    public GameObject AlertPanel;
    public MeshRenderer art3d;

    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                CheckUserSignInStatus();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    void CheckUserSignInStatus()
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                Debug.Log("User is signed in and email is verified.");
            }
            else
            {

                Debug.Log("User is signed in but email is not verified.");
                SendEmailVerification(user);
                // Destroy all previous children (previously instantiated models)
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                // If art3d is a MeshRenderer
                art3d.gameObject.SetActive(false);

                AlertPanel.SetActive(true);
                Application.Quit();
            }
        }
        else
        {
            Debug.Log("No user is signed in.");
            SceneManager.LoadScene("Welcome-Screen");
        }
    }

    void SendEmailVerification(FirebaseUser user)
    {
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                return;
            }
            else
            {
                Debug.Log("Email verification sent successfully.");
            }
        });
    }
}