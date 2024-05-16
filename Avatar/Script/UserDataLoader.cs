using TMPro;
using UnityEngine;

public class UserDataLoader : MonoBehaviour
{
    public TMP_Text profileEmailText;
    public TMP_Text profileUsernameText;
    public TMP_Text profileDistanceText;
    public TMP_Text profileTaskText;
    public TMP_Text profileLandmarkText;
    public TMP_Text profileDateStartText;

    private void Start()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            // Call the GetUserData method from the FirebaseController instance
            FirebaseController.Instance.GetUserData(FirebaseController.Instance.user.UserId, DisplayUserData);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    // Method to display user data
    private void DisplayUserData(string email, string username, int distance, int task, int landmark, string datestart)
    {
        // Display the data in Unity UI
        profileEmailText.text = email;
        profileUsernameText.text = username;
        profileDistanceText.text = distance.ToString() + " km";
        profileTaskText.text = task.ToString();
        profileLandmarkText.text = landmark.ToString();
        profileDateStartText.text = datestart;
    }
}
