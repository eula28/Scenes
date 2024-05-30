using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Google;
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

    async void CheckUserSignInStatus()
    {
        user = auth.CurrentUser;
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                Debug.Log("User is signed in and email is verified.");
                string gender_model = "";
                string bday = "";
                string gender = "";
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                string userId = FirebaseController.Instance.auth.CurrentUser.UserId;
                DocumentReference userDocRef = db.Collection("users").Document(userId);

                DocumentSnapshot doc = await userDocRef.GetSnapshotAsync();
                if (doc.Exists)
                {
                    // Access the data from the document
                    gender_model = doc.GetValue<string>("gender model");
                    bday = doc.GetValue<string>("bday");
                    gender = doc.GetValue<string>("gender");
                }
                else
                {
                    Debug.Log("Document does not exist for user: " + userId);
                }
                if (gender_model == "" || bday == "" || gender == "")
                {
                    SceneManager.LoadScene("GenderSelection");
                }
            }
            else
            {

                Debug.Log("User is signed in but email is not verified.");
                // Destroy all previous children (previously instantiated models)
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
            LogOut();
            SceneManager.LoadScene("Welcome-Screen");
        }
    }

    public void QuitApp()
    {
        LogOut();
        Application.Quit();
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
    }
}