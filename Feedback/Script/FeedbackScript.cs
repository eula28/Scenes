using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button[] stars;
    public TMP_InputField feedbackEmail, feedbackLocation, feedbackComment;
    public int feedbackRating = 0;

    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            int index = i + 1;
            stars[i].onClick.AddListener(() => StarClicked(index));
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
        if (FirebaseController.Instance != null)
        {
            if (string.IsNullOrEmpty(feedbackEmail.text) && string.IsNullOrEmpty(feedbackLocation.text) && string.IsNullOrEmpty(feedbackComment.text) && feedbackRating == 0)
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
                    { "email", feedbackEmail.text },
                    { "location", feedbackLocation.text },
                    { "feedback rating", feedbackRating },
                    { "feedback comment", feedbackComment.text },
                    { "date start", formattedDateTime }
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
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

}