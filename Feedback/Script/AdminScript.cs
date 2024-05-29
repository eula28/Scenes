using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminScript : MonoBehaviour
{
    FirebaseFirestore db;
    public TMP_Dropdown locationDropdown;
    public GameObject feedbackEntryPrefab;
    public Transform feedbackContent;  // Parent transform for instantiated prefabs
    public TMP_Text rating5, rating4, rating3, rating2, rating1;
    private Dictionary<int, int> ratingCounts;

    // Start is called before the first frame update
    void Start()
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            db = FirebaseFirestore.DefaultInstance;
            locationDropdown.onValueChanged.AddListener(delegate { LocationDropdownValueChanged(locationDropdown); });

            // Trigger initial load based on default selected value in dropdown
            LocationDropdownValueChanged(locationDropdown);
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    void LocationDropdownValueChanged(TMP_Dropdown dropdown)
    {
        string selectedLocation = dropdown.options[dropdown.value].text;
        Debug.Log($"Location selected: {selectedLocation}");
        LoadFeedbackForLocation(selectedLocation);
    }

    void LoadFeedbackForLocation(string location)
    {
        db.Collection("feedback").WhereEqualTo("location", location).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting documents: " + task.Exception);
                return;
            }

            // Clear existing feedback entries
            foreach (Transform child in feedbackContent)
            {
                Destroy(child.gameObject);
            }

            ratingCounts = new Dictionary<int, int>();

            foreach (DocumentSnapshot document in task.Result.Documents)
            {
                Dictionary<string, object> feedbackData = document.ToDictionary();

                if (feedbackData.ContainsKey("username") && feedbackData.ContainsKey("feedback rating") && feedbackData.ContainsKey("feedback comment"))
                {
                    
                    string username = feedbackData["username"].ToString();
                    int rating = int.Parse(feedbackData["feedback rating"].ToString());
                    string comment = feedbackData["feedback comment"].ToString();

                    Debug.Log("Getting info: " + username);
                    Debug.Log("Getting info: " + rating);
                    Debug.Log("Getting info: " + comment);

                    // Instantiate a new feedback entry prefab and set its data
                    GameObject feedbackEntry = Instantiate(feedbackEntryPrefab, feedbackContent);
                    feedbackEntry.transform.Find("UsernameText").GetComponent<Text>().text = username;
                    feedbackEntry.transform.Find("CommentText").GetComponent<Text>().text = comment;

                    // Set star images based on the rating
                    SetStarsColor(feedbackEntry, rating);

                    if (!ratingCounts.ContainsKey(rating))
                    {
                        ratingCounts[rating] = 0;
                    }
                    ratingCounts[rating]++;
                }
                else
                {
                    Debug.LogWarning("Document missing required fields: " + document.Id);
                }
            }

            DisplayRatingSummary();
        });
    }

    void SetStarsColor(GameObject feedbackEntry, int rating)
    {
        Color greenColor = Color.green;
        for (int i = 1; i <= 5; i++)
        {
            string starName = $"Star{i}";
            Image starImage = feedbackEntry.transform.Find(starName).GetComponent<Image>();
            if (i <= rating)
            {
                starImage.color = greenColor;
            }
            else
            {
                starImage.color = Color.white;  // Assuming white is the default color for stars
            }
        }
    }

    void DisplayRatingSummary()
    {
        int rating1Count = ratingCounts.ContainsKey(1) ? ratingCounts[1] : 0;
        int rating2Count = ratingCounts.ContainsKey(2) ? ratingCounts[2] : 0;
        int rating3Count = ratingCounts.ContainsKey(3) ? ratingCounts[3] : 0;
        int rating4Count = ratingCounts.ContainsKey(4) ? ratingCounts[4] : 0;
        int rating5Count = ratingCounts.ContainsKey(5) ? ratingCounts[5] : 0;

        rating1.text = rating1Count.ToString();
        rating2.text = rating2Count.ToString();
        rating3.text = rating3Count.ToString();
        rating4.text = rating4Count.ToString();
        rating5.text = rating5Count.ToString();

        // Debug logs can stay if you want to see them in the console
        Debug.Log($"Rating 1: {rating1Count}");
        Debug.Log($"Rating 2: {rating2Count}");
        Debug.Log($"Rating 3: {rating3Count}");
        Debug.Log($"Rating 4: {rating4Count}");
        Debug.Log($"Rating 5: {rating5Count}");
    }
}
