using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class AchievementScript : MonoBehaviour
{
    public void UpdateUserAchievements(string userId, Dictionary<string, object> userActions)
    {
        // Check if the FirebaseController instance exists
        if (FirebaseController.Instance != null)
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            DocumentReference userRef = db.Collection("users").Document(userId);

            // Fetch user data
            userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to fetch user data: " + task.Exception);
                    return;
                }

                // Get user document snapshot
                DocumentSnapshot userSnapshot = task.Result;

                // Extract user points from the snapshot
                int currentPoints = 0;
                if (userSnapshot.Exists)
                {
                    Dictionary<string, object> userData = userSnapshot.ToDictionary();
                    if (userData.ContainsKey("points"))
                    {
                        currentPoints = (int)(long)userData["points"]; // Firestore returns long for integer types
                    }
                }

                // Get userAchievements subcollection
                CollectionReference userAchievementsRef = userRef.Collection("userAchievements");

                // Iterate through user actions
                foreach (var action in userActions)
                {
                    string achievementId = action.Key;
                    int actionCount = (int)action.Value;

                    // Check if achievement exists
                    DocumentReference achievementDocRef = userAchievementsRef.Document(achievementId);
                    achievementDocRef.GetSnapshotAsync().ContinueWithOnMainThread(achievementTask =>
                    {
                        if (achievementTask.Result.Exists)
                        {
                            Dictionary<string, object> achievementData = achievementTask.Result.ToDictionary();

                            // Check if achievement criteria are met
                            if (achievementData.ContainsKey("Criteria") && actionCount >= ((List<object>)achievementData["Criteria"]).Count)
                            {
                                // Check if achievement is not already achieved
                                bool isAchieved = false;
                                if (achievementData.ContainsKey("achieved"))
                                {
                                    isAchieved = (bool)achievementData["achieved"];
                                }

                                if (!isAchieved)
                                {
                                    // Update progress
                                    int progress = actionCount;

                                    // Update or create achievement document
                                    Dictionary<string, object> updateData = new Dictionary<string, object>
                                    {
                                        { "achieved", true },
                                        { "progress", progress }
                                    };

                                    achievementDocRef.SetAsync(updateData, SetOptions.MergeAll);

                                    // Update user points
                                    int pointsToAdd = actionCount; // You can adjust this based on your game logic
                                    int updatedPoints = currentPoints + pointsToAdd;

                                    // Update user points in the database
                                    Dictionary<string, object> userData = new Dictionary<string, object>
                                    {
                                        { "points", updatedPoints }
                                    };

                                    userRef.SetAsync(userData, SetOptions.MergeAll);
                                }
                            }
                        }
                    });
                }
            });
        }
        else
        {
            Debug.LogError("FirebaseController instance not found.");
        }
    }
}
