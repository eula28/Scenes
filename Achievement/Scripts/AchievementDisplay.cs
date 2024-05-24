using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using System;
using Firebase.Extensions;
using System.Linq;

public class AchievementDisplay : MonoBehaviour
{
    public Transform achievementsContainer;
    public GameObject achievementPanelPrefab;
    public TextMeshProUGUI userPointsText;

    string userId;

    void Start()
    {
        InitializeUser();
    }

    void InitializeUser()
    {
        if (FirebaseController.Instance != null)
        {
            userId = FirebaseController.Instance.user.UserId;
            DisplayUserAchievements(userId);
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
            foreach (var userAchievementSnapshot in userAchievementsSnapshots)
            {
                string achievementId = userAchievementSnapshot.Id;
                Dictionary<string, object> userAchievementData = userAchievementSnapshot.ToDictionary();

                FetchAchievementDetails(db, achievementId, userAchievementData);
            }
        });
    }

    void FetchAchievementDetails(FirebaseFirestore db, string achievementId, Dictionary<string, object> userAchievementData)
    {
        DocumentReference achievementRef = db.Collection("achievements").Document(achievementId);

        achievementRef.GetSnapshotAsync().ContinueWithOnMainThread(achievementTask =>
        {
            if (achievementTask.IsFaulted)
            {
                Debug.LogError("Failed to fetch achievement details: " + achievementTask.Exception);
                return;
            }

            DocumentSnapshot achievementSnapshot = achievementTask.Result;
            if (achievementSnapshot.Exists)
            {
                string achievementName = achievementSnapshot.GetValue<string>("Name");
                string description = achievementSnapshot.GetValue<string>("Description");
                int achievementPoints = achievementSnapshot.GetValue<int>("Points");
                int progress = Convert.ToInt32(userAchievementData["Progress"]);
                bool achieved = Convert.ToBoolean(userAchievementData["Achieved"]);

                UpdateUI(achievementName, description, achievementPoints, progress, achieved);
            }
        });
    }

    void UpdateUserPointsUI(int userPoints)
    {
        userPointsText.text = userPoints.ToString();
    }

    void UpdateUI(string achievementName, string description, int achievementPoints, int progress, bool achieved)
    {
        GameObject achievementPanel = Instantiate(achievementPanelPrefab, achievementsContainer);
        Debug.Log("Prefab instantiated: " + achievementPanel.name); // Add this line for debugging
        TextMeshProUGUI[] textFields = achievementPanel.GetComponentsInChildren<TextMeshProUGUI>();

        if (textFields.Length >= 4)
        {
            textFields[0].text = achievementName;
            textFields[1].text = description;
            textFields[2].text = $"Points\n {achievementPoints}";
            textFields[3].text = $"{progress}";
        }
        else
        {
            Debug.LogError("Not enough TMP_Text components found in achievementPanelPrefab.");
        }

        Image panelImage = achievementPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = achieved ? Color.green : Color.red;
        }
        else
        {
            Debug.LogError("Image component not found in achievementPanelPrefab.");
        }
    }
}
