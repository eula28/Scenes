using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button[] stars;
    public TMP_InputField feedbackUsername, feedbackLocation, feedbackComment;
    public int feedbackRating = 0;
    string userId, username;
    FirebaseFirestore db;

    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i + 1;
            stars[i].onClick.AddListener(() => StarClicked(index));
        }

        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            db = FirebaseFirestore.DefaultInstance;
            _ = FetchUsername(userId);
            feedbackLocation.text = PlayerPrefs.GetString("location");
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    private Color STAR_COLOR = Color.green;


    public void StarClicked(int rating)
    {
        feedbackRating = rating;
        for (int i = 0; i < stars.Length; i++)
        {
            Image starImage = stars[i].GetComponent<Image>();
            Color color = i < rating ? STAR_COLOR : Color.white;
            starImage.color = color;
        }
        Debug.Log("Star Rated: " + feedbackRating);
    }

    public void submitFeedback()
    {
        if (string.IsNullOrEmpty(feedbackUsername.text) || string.IsNullOrEmpty(feedbackLocation.text) || string.IsNullOrEmpty(feedbackComment.text) || feedbackRating == 0)
        {
            FirebaseController.Instance.ShowAlert("Kindly fill out all required fields.");
            return;
        }
        else
        {
            System.DateTime currentDateTime = System.DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("MM/dd/yyyy");
            Dictionary<string, object> userFeedback = new Dictionary<string, object>
                {
                    { "user id", FirebaseController.Instance.auth.CurrentUser.UserId },
                    { "username", feedbackUsername.text },
                    { "location", feedbackLocation.text },
                    { "feedback rating", feedbackRating },
                    { "feedback comment", feedbackComment.text },
                    { "date", formattedDateTime }
                };

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            CollectionReference feedbackCollectionRef = db.Collection("feedback");

            feedbackCollectionRef.AddAsync(userFeedback).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to submit feedback: " + task.Exception);
                    return;
                }
                else
                {
                    SceneManager.LoadScene("IR1");
                    Debug.Log("Feedback submitted successfully for user: " + FirebaseController.Instance.auth.CurrentUser.UserId);
                }
            });
        }
    }

    private async Task FetchUsername(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userRef = db.Collection("users").Document(userId);
        DocumentSnapshot userSnapshot = await userRef.GetSnapshotAsync();
        if (userSnapshot.Exists)
        {
            username = userSnapshot.GetValue<string>("username");
            feedbackUsername.text = username;
            Debug.Log("Username found: " + username);
        }
        else
        {
            Debug.LogError("User document does not exist.");
        }
    }

}