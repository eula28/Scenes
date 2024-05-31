using Firebase.Auth;
using Google;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UserDataLoader : MonoBehaviour
{
    public TMP_Text profileEmailText;
    public TMP_Text profileUsernameText;
    public TMP_Text profileDiscoveriesText;
    public TMP_Text profileTaskText;
    public TMP_Text profileDateStartText;
    string ugender_model, ugender, ubday;
    private FirebaseAuth auth; // Initialize this variable
    private FirebaseUser user;
    public GameObject AlertPanel;
    public MeshRenderer art3d;

    private void Start()
    {
        // Initialize the FirebaseAuth object
        auth = FirebaseAuth.DefaultInstance;

        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            // Call the GetUserData method from the FirebaseController instance
            FirebaseController.Instance.GetUserData(FirebaseController.Instance.user.UserId, DisplayUserData);
            CheckUserSignInStatus();
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    // Method to display user data
    private void DisplayUserData(string email, string username, int discoveries, int task, string datestart, string gender_model, string bday, string gender)
    {
        // Display the data in Unity UI
        profileEmailText.text = email;
        profileUsernameText.text = username;
        profileDiscoveriesText.text = discoveries.ToString();
        profileTaskText.text = task.ToString();
        profileDateStartText.text = datestart;
        ugender_model = gender_model;
        ubday = bday;
        ugender = gender;
    }

    void CheckComplete()
    {
        if (ugender_model == "" || ubday == "" || ugender == "")
        {
            SceneManager.LoadScene("GenderSelection");
        }
        else
        {
            Debug.Log("Profile completed.");
        }
    }

    void CheckUserSignInStatus()
    {
        // Get the current user from FirebaseAuth
        user = auth.CurrentUser;
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                Debug.Log("User is signed in and email is verified.");
                // Call CheckComplete if the user is signed in and email is verified
                CheckComplete();
            }
            else
            {
                Debug.Log("User is signed in but email is not verified.");
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }
                // If art3d is a MeshRenderer
                art3d.gameObject.SetActive(false);
                AlertPanel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No user is signed in.");
        }
    }

    public void LogOut()
    {
        // Sign out Firebase and Google users
        if (auth != null && auth.CurrentUser != null)
        {
            auth.SignOut();
        }
        if (GoogleSignIn.DefaultInstance != null)
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        SceneManager.LoadScene("Welcome-Screen");
    }

    public void QuitApp()
    {
        LogOut();
        Application.Quit();
    }
}
