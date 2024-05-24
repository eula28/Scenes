using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using Firebase.Extensions;
using System.Linq;
using System.Collections;

public class AchievementDisplay : MonoBehaviour
{
    public Transform achievementsContainer;
    public GameObject achievementPanelPrefab;
    public TextMeshProUGUI userPointsText;

    string userId;

    void Start()
    {
        Debug.Log("Start method called.");
        InitializeUser();
    }

    void InitializeUser()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            DisplayUserAchievements(userId);
            Debug.Log("Checked.");
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }

    void DisplayUserAchievements(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userRef = db.Collection("users").Document(userId);

        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch user data: " + task.Exception);
                return;
            }

            DocumentSnapshot userSnapshot = task.Result;
            if (!userSnapshot.Exists)
            {
                Debug.LogWarning("User not found.");
                return;
            }

            int userPoints = userSnapshot.GetValue<int>("points");
            Debug.Log("Checked1.");
            UpdateUserPointsUI(userPoints);

            FetchUserAchievements(db, userRef);
        });
    }

    void FetchUserAchievements(FirebaseFirestore db, DocumentReference userRef)
    {
        CollectionReference userAchievementsRef = userRef.Collection("userAchievements");

        userAchievementsRef.GetSnapshotAsync().ContinueWithOnMainThread(userAchievementsTask =>
        {
            if (userAchievementsTask.IsFaulted)
            {
                Debug.LogError("Failed to fetch user achievements: " + userAchievementsTask.Exception);
                return;
            }

            List<DocumentSnapshot> userAchievementsSnapshots = userAchievementsTask.Result.Documents.ToList();
            StartCoroutine(UpdateUIAfterFetch(userAchievementsSnapshots));
        });
    }

    IEnumerator UpdateUIAfterFetch(List<DocumentSnapshot> userAchievementsSnapshots)
    {
        foreach (var userAchievementSnapshot in userAchievementsSnapshots)
        {
            string achievementId = userAchievementSnapshot.Id;
            Dictionary<string, object> userAchievementData = userAchievementSnapshot.ToDictionary();

            yield return FetchAchievementDetailsCoroutine(achievementId, userAchievementData);
            Debug.Log("CheckedFetchUser.");
        }
    }

    IEnumerator FetchAchievementDetailsCoroutine(string achievementId, Dictionary<string, object> userAchievementData)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference achievementRef = db.Collection("achievements").Document(achievementId);

        var task = achievementRef.GetSnapshotAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Failed to fetch achievement details: " + task.Exception);
            yield break;
        }

        DocumentSnapshot achievementSnapshot = task.Result;
        if (achievementSnapshot.Exists)
        {
            string achievementName = achievementSnapshot.GetValue<string>("Name");
            string description = achievementSnapshot.GetValue<string>("Description");
            int achievementPoints = achievementSnapshot.GetValue<int>("Points");

            // Check if "Progress" key exists in userAchievementData
            if (!userAchievementData.ContainsKey("Progress"))
            {
                // If "Progress" key doesn't exist, add it with a default value
                userAchievementData["Progress"] = 0; // You can replace 0 with any default value you want
            }
            // Check if "Progress" key exists in userAchievementData
            if (!userAchievementData.ContainsKey("Achieved"))
            {
                // If "Progress" key doesn't exist, add it with a default value
                userAchievementData["Achieved"] = 0; // You can replace 0 with any default value you want
            }

            // Fetch the progress value from userAchievementData
            int progress = Convert.ToInt32(userAchievementData["Progress"]);
            bool achieved = Convert.ToBoolean(userAchievementData["Achieved"]);

            UpdateUI(achievementName, description, achievementPoints, progress, achieved);
            Debug.Log("Progress: " + userAchievementData["Progress"]);
            Debug.Log("Achieved: " + userAchievementData["Achieved"]);
        }
    }


    void UpdateUserPointsUI(int userPoints)
    {
        userPointsText.text = userPoints.ToString();
        Debug.Log("Checked:P.");
    }

    void UpdateUI(string achievementName, string description, int achievementPoints, int progress, bool achieved)
    {
        GameObject achievementPanel = Instantiate(achievementPanelPrefab, achievementsContainer);
        Debug.Log("Prefab instantiated: " + achievementPanel.name);
        TextMeshProUGUI[] textFields = achievementPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (textFields.Length >= 4)
        {
            textFields[0].text = achievementName;
            textFields[1].text = description;
            textFields[2].text = $"Points\n {achievementPoints}";
            textFields[3].text = $"{progress}";
            Debug.Log("Checkeddddd.");
        }
        else
        {
            Debug.LogError("Not enough TMP_Text components found in achievementPanelPrefab.");
        }

        Image panelImage = achievementPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            Debug.Log("ColorCheckeddddd.");
           

            panelImage.color = achieved ? Color.red : Color.white;
        }
        else
        {
            Debug.LogError("Image component not found in achievementPanelPrefab.");
        }
    }
}
